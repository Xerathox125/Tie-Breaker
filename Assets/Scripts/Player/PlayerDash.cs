using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Config")]
    public float dashForce;                    // Potencia del impulso
    public float dashDuration;                 // Duración del dash
    public float coolDownDash;                 // Tiempo de enfriamiento
    public bool isDash = false;                // Indica si el jugador está en estado dash

    [Header("Timers")]
    public float timerDashDuration = 0f;       // Tiempo restante del dash actual
    public float timerCoolDownDash = 0f;       // Tiempo restante para enfriamiento

    private Vector2 dashDirection;             // Dirección en la que se aplicará el dash
    private PlayerController playerController; // Referencia al controlador
    private Damageable damageable;             // Referencia al componente Damageable
    [SerializeField] private LayerMask enemyLayer; // Asigna aquí la capa de los enemigos
    private int playerLayer;

    //Getters y setters
    public bool hasAirDashed { get; set; }

    private void Start() // Cachea controlador
    { 
        playerController = GetComponent<PlayerController>();
        damageable = GetComponent<Damageable>();
        playerLayer = gameObject.layer; // Guarda la capa del jugador
    }

    public void OnUpdate() // Actualiza contadores y resets
    {
        if (timerCoolDownDash > 0f) timerCoolDownDash -= Time.deltaTime; // Reduce enfriamiento        
        if (playerController.jump.IsGrounded && !isDash) hasAirDashed = false; // Si toca suelo y no está haciendo dash, reinicia el token del dash aéreo        
    }

    public void OnFixedUpdate() // Ejecuta la lógica física durante el dash
    {
        if (isDash) DashUpdate(); // Llama a la actualización si está activo
    }

    public void DashHold() // Lógica de inicio de dash
    {
        if (isDash || playerController.stairs.IsStairs) return; // Cancela si ya está haciendo dash o en escaleras

        bool canGroundDash = playerController.jump.IsGrounded && timerCoolDownDash <= 0f;
        bool canAirDash = !playerController.jump.IsGrounded && !hasAirDashed; 

        if (canGroundDash || canAirDash) DashStart();
    }

    void DashStart() // Configura el estado inicial del dash
    {
        isDash = true;
        timerDashDuration = dashDuration; // Resetea duración
        playerController.rb.gravityScale = 0f; // Elimina gravedad temporalmente
        
        Vector2 move = playerController.moveInput; // Lee input actual

        // dash horizontal
        float horizontalInput = Mathf.Abs(move.x) > 0.1f ? Mathf.Sign(move.x) : Mathf.Sign(transform.localScale.x);
        dashDirection = new Vector2(horizontalInput, 0f);

        //dash omnidireccional
        //dashDirection = move.magnitude > 0.1f ? move.normalized : new Vector2(Mathf.Sign(transform.localScale.x), 0f);


        // Lógica de consumo de token
        if (playerController.jump.IsGrounded) timerCoolDownDash = coolDownDash;
        else hasAirDashed = true; // Aquí se marca como usado

        // Si dash es vertical, también consume token
        //if (dashDirection.y > 0.1f) hasAirDashed = true;

        // Se activa invulnerabilidad durante el dash
        if (damageable != null)
        {
            damageable.TriggerInvulnerability(dashDuration);
        }
        //int enemyLayerIndex = Mathf.RoundToInt(Mathf.Log(enemyLayer.value, 2));
        //Physics2D.IgnoreLayerCollision(playerLayer, enemyLayerIndex, true);
    }

    void DashUpdate() // Aplica movimiento constante mientras dura el dash
    {
        playerController.rb.linearVelocity = dashDirection * dashForce; // Aplica velocidad
        timerDashDuration -= Time.deltaTime; // Reduce tiempo restante

        if (timerDashDuration <= 0) DashEnd(); // Finaliza si el tiempo termina
    }

    void DashEnd() // Finaliza el estado de dash
    {
        isDash = false; // Desactiva estado
        playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad
        playerController.rb.linearVelocity = Vector2.zero; // Detiene movimiento residual
        //int enemyLayerIndex = Mathf.RoundToInt(Mathf.Log(enemyLayer.value, 2));
        //Physics2D.IgnoreLayerCollision(playerLayer, enemyLayerIndex, false);
    }
}
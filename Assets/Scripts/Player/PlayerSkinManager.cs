using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Controladores de Animación para los 3 Skins")]
    [SerializeField] private RuntimeAnimatorController skin1Controller; // MarimondaAnims1 y 2
    [SerializeField] private RuntimeAnimatorController skin2Controller; // MarimondaAnims3 y 4
    [SerializeField] private RuntimeAnimatorController skin3Controller; // MarimondaAnims5 y 6

    private void Start()
    {
        string skinType = ScenesManager.GetCurrentPlayerSkin();
        Debug.Log("--- [PlayerSkinManager] Inicio detectado en el objeto: " + gameObject.name + " ---");
        Debug.Log("Skin devuelto por ScenesManager: " + skinType);

        ApplySkin(skinType);
    }

    private void ApplySkin(string skinType)
    {
        Animator anim = GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogError("[PlayerSkinManager] ¡No se encontró el componente Animator en el objeto Player!");
            return;
        }

        switch (skinType)
        {
            case "Skin1":
                if (skin1Controller != null)
                {
                    anim.runtimeAnimatorController = skin1Controller;
                    Debug.Log("[PlayerSkinManager] Skin 1 aplicado correctamente.");
                }
                else Debug.LogWarning("[PlayerSkinManager] skin1Controller está vacío (None).");
                break;

            case "Skin2":
                if (skin2Controller != null)
                {
                    anim.runtimeAnimatorController = skin2Controller;
                    Debug.Log("[PlayerSkinManager] Skin 2 (Override) aplicado correctamente.");
                }
                else Debug.LogWarning("[PlayerSkinManager] skin2Controller está vacío (None).");
                break;

            case "Skin3":
                if (skin3Controller != null)
                {
                    anim.runtimeAnimatorController = skin3Controller;
                    Debug.Log("[PlayerSkinManager] Skin 3 (Override) aplicado correctamente.");
                }
                else Debug.LogWarning("[PlayerSkinManager] skin3Controller está vacío (None).");
                break;

            default:
                if (skin1Controller != null)
                {
                    anim.runtimeAnimatorController = skin1Controller;
                    Debug.Log("[PlayerSkinManager] Skin por defecto (Skin1) aplicado.");
                }
                break;
        }
    }
}
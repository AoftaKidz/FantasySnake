using TMPro;
using UnityEngine;

public class UIHeroStatus : MonoBehaviour
{
    [SerializeField] TextMeshPro txtHP;
    [SerializeField] TextMeshPro txtDef;
    [SerializeField] TextMeshPro txtAtk;
    [SerializeField] CharacterStatus status;

    private void Update()
    {
        if (status == null) return;

        txtHP.text = $"{status.health}/{status.maxHealth}";
        txtDef.text = $"{status.defense}";
        txtAtk.text = $"{status.attack}";
    }
}

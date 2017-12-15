using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] Button m_PreviousButton = null;
    [SerializeField] Button m_NextButton = null;
    [SerializeField] TextMeshProUGUI m_SelectedUnitNameText = null;

    public Button GetPreviousButton() { return m_PreviousButton; }
    public Button GetNextButton() { return m_NextButton; }
    public TextMeshProUGUI GetSelectedUnitNameText() { return m_SelectedUnitNameText; }
}
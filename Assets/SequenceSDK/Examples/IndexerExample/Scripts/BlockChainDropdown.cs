using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class BlockChainDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown = null;

    /// <summary>
    /// Returns the new value selected from the dropdown as BlockChainType
    /// </summary>
    public UnityAction<BlockChainType> OnDropdownValueChange;

    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Awake()
    {
        if(dropdown == null)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        List<string> options = new List<string>();

        foreach (string blockChainName in System.Enum.GetNames(typeof(BlockChainType)))
        {
            options.Add(blockChainName);
        }

        dropdown.AddOptions(options);
    }

    /// <summary>
    /// Returns the current selected option as BlockChainType
    /// </summary>
    /// <returns></returns>
    public BlockChainType GetSelectedOption()
    {
        return (BlockChainType)dropdown.value;
    }

    /// <summary>
    /// Subscribbed to <see cref="dropdown"/> when the value changes
    /// </summary>
    /// <param name="index"></param>
    private void OnDropdownValueChanged(int index)
    {
        OnDropdownValueChange?.Invoke(GetSelectedOption());
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SequenceSharp;
using BigInteger = System.Numerics.BigInteger;
using System.Text;

/*

Clicking on a Category will slide menu over to left and show content in a panel from the right
Categories are sorted into sub menus based on Contract Type

*/
[System.Serializable]
public class Category : MonoBehaviour
{
    [SerializeField]
    private Button catBtn = null;

    [SerializeField]
    private Image iconImg = null;

    [SerializeField]
    private TextMeshProUGUI buttonLabel = null;

    public string _catName { get; private set; }
    public string _contractAddress { get; private set; }
    public Texture _icon { get; private set; }
    public ContractType _contractType { get; private set; }

    private BigInteger _tokenID;

    private void OnDisable()
    {
        catBtn.onClick.RemoveListener(OnButtonClick);
    }

    private void Start()
    {
        if (catBtn == null)
        {
            catBtn = GetComponent<Button>();
        }

        catBtn.onClick.AddListener(OnButtonClick);
    }

    public void Init(
        string name,
        Texture icon,
        ContractType contractType,
        string contractAddress,
        BigInteger? tokenID
    )
    {
        _catName = name;
        this._icon = icon;
        this._contractType = contractType;
        this._contractAddress = contractAddress;
        if (tokenID != null)
        {
            this._tokenID = (BigInteger)tokenID;
        }
        buttonLabel.text = name;

        if (icon != null)
        {
            ApplyIcon(icon);
        }
        else
        {
            iconImg.gameObject.SetActive(false);
            //StretchLabel();
        }
    }

    public Button GetButton() { return catBtn; }
    public TextMeshProUGUI GetLabel() { return buttonLabel; }
    public Image GetImage() { return iconImg; }

    private async void OnButtonClick()
    {
        Debug.Log("contract type:" + _contractType);

        Debug.Log("contract address:" + _contractAddress);

        var web3 = DemoManager.Instance.web3;
        var address = (await web3.Eth.Accounts.SendRequestAsync())[0];
        if (_contractType == ContractType.ERC20)
        {
            var contract = new ERC20(web3, _contractAddress);
            await contract.Transfer(DemoManager.exampleToAccount, BigInteger.One);
        }
        else if (_contractType == ContractType.ERC721)
        {
            var contract = new ERC721(web3, _contractAddress);
            await contract.SafeTransferFrom(address, DemoManager.exampleToAccount, _tokenID);
        }
        else if (_contractType == ContractType.ERC1155)
        {
            var contract = new ERC1155(web3, _contractAddress);
            await contract.SafeTransferFrom(
                address,
                DemoManager.exampleToAccount,
                _tokenID,
                BigInteger.One,
                Encoding.ASCII.GetBytes("")
            );
        }
        else
        {
            Debug.LogWarning($"Can't send asset for contract type {_contractType}");
        }
    }

    private void ApplyIcon(Texture tex)
    {
        if (tex != null)
        {
            float width = iconImg.rectTransform.rect.width;
            float height = iconImg.rectTransform.rect.height;
            iconImg.sprite = Sprite.Create(
                TextureHelper.ConvertToTexture2D(tex, (int)width, (int)height),
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }
    }
}

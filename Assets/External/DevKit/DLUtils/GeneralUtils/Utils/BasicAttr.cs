using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PRSTransform
{
    public string position;
    public string rotation;
    public string scale;


    public PRSTransform()
    {
        position = Vector3.zero.ToString();
        rotation = Quaternion.identity.ToString();
        scale = Vector3.one.ToString();
    }
    public static PRSTransform Zero()
    {
        var t =  new PRSTransform();
        t.scale = Vector3.zero.ToString();
        return t;
    }

    public PRSTransform(Transform t)
    {
        UpdatePRSTransform(t);
    }

    public void UpdatePRSTransform(Transform t)
    {
        position = t.position.ToString();
        rotation = t.rotation.ToString();
        scale = t.localScale.ToString();
    }

    public void RecoverPRSTransform(Transform t)
    {
        t.position = PHUtils.GetVec3ByString(position);
        t.rotation = PHUtils.GetQuaByString(rotation);
        t.localScale = PHUtils.GetVec3ByString(scale);
    }

    public Vector3 GetPosition()
    {
        return PHUtils.GetVec3ByString(position);
    }
    public Quaternion GetRotation()
    {
        return PHUtils.GetQuaByString(rotation);
    }
    public Vector3 GetScale()
    {
        return PHUtils.GetVec3ByString(scale);
    }

    public PRSTransform Copy()
    {
        PRSTransform prs = new PRSTransform();
        prs.position = this.position;
        prs.rotation = this.rotation;
        prs.scale = this.scale;
        return prs;
    }
}
[Serializable]
public class AddressableBasicItemWithAmount : AddressableBasicItem
{
    public int amount = 1;
    public AddressableBasicItemWithAmount(int assetsCode, int amount)
    {
        this.assetsCode = assetsCode;
        this.amount = amount;
    }
}
[Serializable]
public class AddressableBasicItem
{
    public int assetsCode = -1;
    public string modelName = "";
    public string modelUrl = "";
    public string configUrl = "";
    public string imageUrl = "";

    public int type = 0; //0 for aa, 1 for config
    public string author = "";
    private AuthorInfo authorInfo;

    [JsonIgnore]
    public string authorName
    {
        get
        {
            if(author==null || author.Contains("null")) return "";
            if (authorInfo == null && !string.IsNullOrEmpty(author))
            {
                authorInfo = PHUtils.ConvertStringToObject<AuthorInfo>(author);
            }
            if (authorInfo == null) return "";
            return authorInfo.nickname;
        }
    }

    public AddressableBasicItem(int assetsCode, string modelName, string modelUrl)
    {
        this.assetsCode = assetsCode;
        this.modelName = modelName;
        this.modelUrl = modelUrl;
    }
    public AddressableBasicItem(int assetsCode, string modelName, string modelUrl, string configUrl)
    {
        this.assetsCode = assetsCode;
        this.modelName = modelName;
        this.modelUrl = modelUrl;
        this.configUrl = configUrl;
    }
    public AddressableBasicItem(int assetsCode, string modelName, string modelUrl, string configUrl, string imageUrl)
    {
        this.assetsCode = assetsCode;
        this.modelName = modelName;
        this.modelUrl = modelUrl;
        this.configUrl = configUrl;
        this.imageUrl = imageUrl;
    }
    [JsonConstructor]
    public AddressableBasicItem(int assetsCode)
    {
        this.assetsCode = assetsCode;
    }
    public AddressableBasicItem()
    {

    }
    public bool isEqual(AddressableBasicItem address)
    {
        bool result = false;
        if (address != null)
        {
            if (address.assetsCode == this.assetsCode && address.assetsCode == this.assetsCode && address.assetsCode == this.assetsCode && address.assetsCode == this.assetsCode)
            {
                result = true;
            }
        }
        return result;
    }
    [Serializable]
    public class AuthorInfo
    {
        public int id;
        public string nickname;
        public string avatarUrl;

        public AuthorInfo()
        {
        }

        public AuthorInfo(int id, string nickname, string avatarUrl)
        {
            this.id = id;
            this.nickname = nickname;
            this.avatarUrl = avatarUrl;
        }
    }

    void test()
    {
        AuthorInfo auhtorInfo = new AuthorInfo();
        auhtorInfo.id = 1;
        auhtorInfo.nickname = "Lin";
        auhtorInfo.avatarUrl = "https://";

        string author = PHUtils.ConvertObject2String(auhtorInfo);



    }

}


public enum AssetFormat
{
    AB,
    GLB,
    VRM,
    BVH,
    PaperPerson,
    PolyUrl,
    Other,//FBX,ZIP等其他格式
}


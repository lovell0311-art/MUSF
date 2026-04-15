using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [ObjectSystem]
    public class SafeAreaComponentAwake : AwakeSystem<SafeAreaComponent>
    {
        public override void Awake(SafeAreaComponent self)
        {
            SafeAreaComponent.Instances = self;
            AssetBundleComponent.Instance.LoadBundle(self.SafesResName.StringToAB());
            GameObject  safes= (GameObject)AssetBundleComponent.Instance.GetAsset(self.SafesResName.StringToAB(),self.SafesResName);
            self.collector=safes.GetReferenceCollector();
        }
    }
}
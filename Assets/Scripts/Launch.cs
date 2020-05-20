using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Launch : MonoBehaviour
{
    List<Action> DllUIUpdateList = new List<Action>();

    ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    void Start()
    {
        
        StartCoroutine(LoadILRuntime());
    }
   

    IEnumerator LoadILRuntime()
    {
        //读取dll文件
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        WWW www = new WWW(Application.streamingAssetsPath + "/Hotfix.dll");

        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        byte[] dll = www.bytes;
        www.Dispose();

        www = new WWW(Application.streamingAssetsPath + "/Hotfix.pdb");

        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        byte[] pdb = www.bytes;
        //这个流 只加载用到 的部分 意思是流要保持全局变量 且需动态加载
        System.IO.MemoryStream fs = new MemoryStream(dll);
        System.IO.MemoryStream p = new MemoryStream(pdb);
        appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

        appdomain.DebugService.StartDebugService(56000);
        //StartCoroutine(WaitForZhiZhang());
        OnILRuntimeInit();
        OnILRuntimeInitialized();
    }
    IEnumerator WaitForZhiZhang()
    {
        while (!appdomain.DebugService.IsDebuggerAttached){
            yield return null;
        }
        yield return new WaitForSeconds(1);
        OnILRuntimeInit();
        OnILRuntimeInitialized();
    }

    void Update()
    {
        Debug.Log("开始执行主工程Update函数");
        if (DllUIUpdateList.Count > 0)
        {
            for(int i=0;i< DllUIUpdateList.Count; i++)
            {
                DllUIUpdateList[i].Invoke();
            }
        }
    }

    void OnILRuntimeInit()
    {
        //跨域继承绑定适配器
        appdomain.RegisterCrossBindingAdaptor(new InterfaceIUIAdaptor());
        //Button点击事件的委托注册
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
    }

    void OnILRuntimeInitialized()
    {
        
        //获取Hotfix.dll内部定义的类
        List<Type> allTypes = new List<Type>();
        var values = appdomain.LoadedTypes.Values.ToList();
        foreach (var v in values)
        {
            allTypes.Add(v.ReflectionType);
        }
        //去重
        allTypes = allTypes.Distinct().ToList();

        DllUIUpdateList.Clear();
        foreach (var v in allTypes)
        {
            //找到实现IUI接口的类 Adaptor 前面写的适配器IUI的类
            if (v.IsClass && v.GetInterface("Adaptor") != null)
            {
                //生成实例
                var gs = appdomain.Instantiate<IUI>(v.FullName);

                //调用接口方法
                gs.Start();
                DllUIUpdateList.Add(gs.Update);
            }
        }
    }
}


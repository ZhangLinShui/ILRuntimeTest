using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using static CoroutineAdapter;

public class InterfaceIUIAdaptor : CrossBindingAdaptor
{

    //public override Type BaseCLRType
    //{
    //    get
    //    {
    //        return typeof(IUI);//这是你想继承的那个类
    //    }
    //}

    //public override Type AdaptorType
    //{
    //    get
    //    {
    //        return typeof(Adaptor);//这是实际的适配器类
    //    }
    //}

    //public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    //{
    //    return new Adaptor(appdomain, instance);//创建一个新的实例
    //}

    ////实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    //public class Adaptor : IUI, CrossBindingAdaptorType
    //{
    //    ILTypeInstance instance;
    //    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    //    IMethod m_Start;
    //    bool m_StartGot;

    //    IMethod m_Update;
    //    bool m_UpdateGot;

    //    public Adaptor()
    //    {

    //    }

    //    public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    //    {
    //        this.appdomain = appdomain;
    //        this.instance = instance;
    //    }

    //    public ILTypeInstance ILInstance { get { return instance; } }

    //    //你需要重写所有你希望在热更脚本里面重写的方法，并且将控制权转到脚本里去
    //    public void Start()
    //    {
    //        if (!m_StartGot)
    //        {
    //            m_Start = instance.Type.GetMethod("Start", 0);
    //            m_StartGot = true;
    //        }
    //        if (m_Start != null)
    //        {
    //            appdomain.Invoke(m_Start, instance, null);//没有参数建议显式传递null为参数列表，否则会自动new object[0]导致GC Alloc
    //        }
    //    }

    //    public void Update()
    //    {
    //        if (!m_UpdateGot)
    //        {
    //            m_Update = instance.Type.GetMethod("Update", 0);
    //            m_UpdateGot = true;
    //        }
    //        if (m_Update != null)
    //        {
    //            appdomain.Invoke(m_Update, instance, null);
    //        }
    //    }
    //}

    //————————————————
    //版权声明：本文为CSDN博主「王王王渣渣」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
    //原文链接：https://blog.csdn.net/wangjiangrong/java/article/details/90294366
    public override Type BaseCLRType => typeof(IUI);//这是你想继承的那个类


    public override Type AdaptorType => typeof(Adaptor);//这是实际的适配器类


    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);//创建一个新的实例

    }//实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : IUI, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        IMethod m_Start;
        bool m_StartGot;

        IMethod m_Update;
        bool m_UpdateGot;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        //你需要重写所有你希望在热更脚本里面重写的方法，并且将控制权转到脚本里去
        public void Start()
        {
            if (!m_StartGot)
            {
                m_Start = instance.Type.GetMethod("Start", 0);
                m_StartGot = true;
            }
            if (m_Start != null)
            {
                appdomain.Invoke(m_Start, instance, null);//没有参数建议显式传递null为参数列表，否则会自动new object[0]导致GC Alloc
            }
        }

        public void Update()
        {
            if (!m_UpdateGot)
            {
                m_Update = instance.Type.GetMethod("Update", 0);
                m_UpdateGot = true;
            }
            if (m_Update != null)
            {
                appdomain.Invoke(m_Update, instance, null);
            }
        }
    }
}

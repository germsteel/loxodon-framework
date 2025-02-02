![](docs/images/icon.png)

# Loxodon Framework(Unity-MVVM)

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE)
[![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework)](https://www.npmjs.com/package/com.vovgou.loxodon-framework)

[(中文版)](README_CN.md)

**MVVM and Databinding for Unity3d(C# & XLua & ILRuntime)**

*Developed by Clark*

Requires Unity 2018.4 or higher.

LoxodonFramework is a lightweight MVVM (Model-View-ViewModel) framework specifically crafted for Unity3D. It includes data binding and various useful components. The framework's performance is meticulously optimized, avoiding value type boxing and unboxing, minimizing garbage collection overhead. It utilizes dynamic delegates/static code weaving techniques to ensure comparable performance between data binding and direct invocation, zero garbage collection during UI view updates, and more. Additionally, it has been validated in projects, demonstrating superior performance, stability, and reliability with a clear, extensible architecture. We hope it can contribute to making your game development faster and more effortless.

For tutorials,examples and support,please see the project.You can also discuss the project in the Unity Forums.

The plugin is compatible with MacOSX,Windows,Linux,UWP,WebGL,IOS and Android,and provides all the source code of the project.

If you like this framework or think it is useful, please write a review on [AssetStore](https://assetstore.unity.com/packages/tools/gui/loxodon-framework-2-0-178583#reviews) or give me a STAR or FORK it on Github, thank you!

**Tested in Unity 3D on the following platforms:**  
PC/Mac/Linux  
IOS  
Android  
UWP(window10)  
WebGL  

## Installation

For detailed installation steps, please refer to the **[installation documentation](Installation.md)**.

## English manual

- [HTML](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.md)
- [PDF](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.pdf)

## Key Features:
- MVVM Framework;
- Multiple platforms;
- Higher Extensibility;
- async&await (C#&Lua)
- try&catch&finally for lua
- XLua support(You can make your game in lua.);
- Asynchronous result and asynchronous task are supported;
- Scheduled Executor and Multi-threading;<br>
- Messaging system support;
- Preferences can be encrypted;
- Localization support;
- Code weaving
- Databinding support:
    - Avoiding boxing and unboxing of value types;
    - Optimizing performance through dynamic delegates/static code weaving techniques, avoiding the use of reflection;
    - Minimizing garbage collection, avoiding memory allocations during string concatenation, and numeric-to-string conversions;
    - Data binding performance is comparable to direct invocation;
    - Zero garbage collection during UI view updates;
    - Field binding;
    - Property binding;
    - Dictionary,list and array binding;
    - Event binding;
    - Unity3d's EventBase binding;
    - Static property and field binding;
    - Method binding;
    - Command binding;
    - ObservableProperty,ObservableDictionary and ObservableList binding;
    - Expression binding;

## Notes  
- .Net2.0 and .Net2.0 Subset,please use version 1.9.x.
- LoxodonFramework 2.0 supports .Net4.x and .Net Standard2.0  
- LoxodonFramework 2.0 supports Mono and IL2CPP 

## Plugins
- [Loxodon Framework OSA](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.OSA)

	This plugin is designed to optimize [Optimized ScrollView Adapter](https://assetstore.unity.com/packages/tools/gui/optimized-scrollview-adapter-68436), specifically adding data binding capabilities to ListView and GridView.

    **Note: This plugin depends on [Optimized ScrollView Adapter](https://assetstore.unity.com/packages/tools/gui/optimized-scrollview-adapter-68436). Please ensure you have installed Optimized ScrollView Adapter before using this plugin.**

- [Loxodon Framework TextFormatting](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextFormatting) 

    This is a text formatting plugin modified based on the official C# library. By extending the AppendFormat function of StringBuilder, it aims to avoid garbage collection (GC) when concatenating strings or converting numbers to strings. This optimization is particularly beneficial in scenarios with high-performance requirements.

    Furthermore, the plugin extends Unity's Unity GUI (UGUI) by introducing two new text controls: TemplateText and FormattableText. These controls support the data binding features of MVVM, allowing the binding of ViewModel or value-type objects to the controls. This approach eliminates the need for boxing and unboxing of value-type objects, thus maximizing the optimization of garbage collection (GC).

    It's worth noting that using the controls TemplateTextMeshPro or FormattableTextMeshProUGUI from Loxodon.Framework.TextMeshPro can further reduce garbage collection (GC), achieving a completely GC-free update of the game view.

- [Loxodon Framework TextMeshPro](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextMeshPro) 

	This plugin primarily serves to enhance AlertDialog and Toast views by providing TextMeshPro support, replacing UnityEngine.UI.Text with TextMeshProUGUI for optimized UI views.

	Additionally, the plugin depends on the Loxodon.Framework.TextFormatting plugin, further optimizing garbage collection. By utilizing FormattableTextMeshProUGUI and TemplateTextMeshProUGUI controls, updating UI views results in absolutely no garbage collection (GC), achieving a fully GC-free view update.

- [Loxodon Framework Data](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Data)

	This plugin supports exporting data from Excel files to Json files, Lua files, or LiteDB databases. Additionally, it enables converting data to C# objects using Json.Net. It is recommended to use LiteDB for storing configuration data, as it is a lightweight NoSQL embedded database that supports ORM functionality, BSON format, and data indexing, making it highly convenient to use.

    - [Loxodon.Framework.Data.LiteDB](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Data/Packages/com.vovgou.loxodon-framework-data-litedb)
    - [Loxodon.Framework.Data.Newtonsoft](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Data/Packages/com.vovgou.loxodon-framework-data-newtonsoft)


- [Loxodon Framework Fody](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Fody)

	This is a plugin for static code weaving, comprising multiple sub-plugins. It supports static weaving for objects, including the addition of the ToString function, integration of the PropertyChanged event, incorporation of the BindingProxy class, and more. This not only optimizes performance but also enhances development efficiency.

    - [Loxodon.Framework.Fody.PropertyChanged](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Fody/Packages/com.vovgou.loxodon-framework-fody-propertychanged)
    - [Loxodon.Framework.Fody.ToString](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Fody/Packages/com.vovgou.loxodon-framework-fody-tostring)
    - [Loxodon.Framework.Fody.BindingProxy](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Fody/Packages/com.vovgou.loxodon-framework-fody-bindingproxy)
    

- [Loxodon Framework UIToolkit](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.UIToolkit)

	This plugin integrates UIToolkit into the Loxodon.Framework, introducing the UIToolkitWindow class. It supports data binding and allows for a mix of UIToolkit and UGUI.

- [Loxodon Framework ILRuntime](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.ILRuntime)

- [Loxodon Framework Localization For CSV](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.LocalizationsForCsv)

    It supports localization files in csv format, requires Unity2018.4 or higher.

- [Loxodon Framework XLua](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.XLua)

    It supports making games with lua scripts.

- [Loxodon Framework Bundle](https://assetstore.unity.com/packages/slug/87419)

    Loxodon Framework Bundle is an AssetBundle manager.It provides a functionality that can automatically manage/load an AssetBundle and its dependencies from local or remote location.Asset Dependency Management including BundleManifest that keep track of every AssetBundle and all of their dependencies. An AssetBundle Simulation Mode which allows for iterative testing of AssetBundles in a the Unity editor without ever building an AssetBundle.

    The asset redundancy analyzer can help you find the redundant assets included in the AssetsBundles.Create a fingerprint for the asset by collecting the characteristic data of the asset. Find out the redundant assets in all AssetBundles by fingerprint comparison.it only supports the AssetBundle of Unity 5.6 or higher.

    ![](docs/images/bundle.png)
- [Loxodon Framework NLog](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.NLog)

    This plug-in integrates NLog into Loxodon.Framework. It is recommended to use this plug-in instead of the Log4Net plug-in. It allocates less heap memory during the log printing process.
- [Loxodon Framework Log4Net](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Log4Net)

    This is a log plugin.It helps you to use Log4Net in the Unity3d.

    ![](docs/images/log4net.png)

- [Loxodon Framework Obfuscation](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Obfuscation)

	The plugin is a data type memory obfuscation tool that supports ObfuscatedByte, ObfuscatedShort, ObfuscatedInt, ObfuscatedLong, ObfuscatedFloat, and ObfuscatedDouble types. Its purpose is to prevent memory modification of game values by memory editors. The plugin supports all arithmetic operators for numerical types and can automatically convert between them and their standard counterparts (byte, short, int, long, float, double).

	During the obfuscation of Float and Double types, the plugin converts them to int and long types for bitwise AND and OR operations to ensure that precision is not lost. Unsafe code is used for type conversion to balance conversion performance. The plugin aims to provide a comprehensive solution for protecting game values against memory modification, allowing for seamless integration with different numerical types and maintaining performance through careful type conversion.

    **Example:**

		ObfuscatedInt  length = 200;
		ObfuscatedFloat scale = 20.5f;
		int offset = 30;
		
		float value = (length * scale) + offset;

- [Loxodon Framework Addressable](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Addressable)

- [Loxodon Framework Connection](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Connection)

    This is a network connection component, implemented using TcpClient, supports IPV6 and IPV4, automatically recognizes the current network when connecting with a domain name, and preferentially connects to the IPV4 network.

- [DotNetty for Unity](https://github.com/vovgou/DotNettyForUnity)

    DotNetty is a port of [Netty](https://github.com/netty/netty), asynchronous event-driven network application framework for rapid development of maintainable high performance protocol servers & clients.

    This version is modified based on [DotNetty](https://github.com/Azure/DotNetty)'s 0.7.5 version and is a customized version for the Unity development platform. It includes the removal of certain dependency libraries, bug fixes, performance optimizations, and enhancements to cater to game development on the Unity platform. Additionally, it has undergone testing under IL2CPP for compatibility and reliability.

- [LiteDB](https://github.com/mbdavid/LiteDB)

    LiteDB is a small, fast and lightweight NoSQL embedded database.

    ![](https://camo.githubusercontent.com/d85fc448ef9266962a8e67f17f6d16080afdce6b/68747470733a2f2f7062732e7477696d672e636f6d2f6d656469612f445f313432727a57774145434a44643f666f726d61743d6a7067266e616d653d39303078393030)

## Quick Start

Create a view and view model of the progress bar.

![](docs/images/progress.png)

    public class ProgressBarViewModel : ViewModelBase {
        private string tip;
        private bool enabled;
        private float value;
        public ProgressBarViewModel() {
        }

        public string Tip {
            get { return this.tip; }
            set { this.Set<string>(ref this.tip, value, nameof(Tip)); }
        }

        public bool Enabled {
            get { return this.enabled; }
            set { this.Set<bool>(ref this.enabled, value, nameof(Enabled)); }
        }

        public float Value {
            get { return this.value; }
            set { this.Set<float>(ref this.value, value, nameof(Value)); }
        }
    }

    public class ProgressBarView : UIView {
        public GameObject progressBar;
        public Text progressTip;
        public Text progressText;
        public Slider progressSlider;

        protected override void Awake() {
            var bindingSet = this.CreateBindingSet<ProgressBar, ProgressBarViewModel>();

            bindingSet.Bind(this.progressBar).For(v => v.activeSelf).To(vm => vm.Enabled).OneWay();
            bindingSet.Bind(this.progressTip).For(v => v.text).To(vm => vm.Tip).OneWay();
            bindingSet.Bind(this.progressText).For(v => v.text)
                .ToExpression(vm => string.Format("{0:0.00}%", vm.Value * 100)).OneWay();
            bindingSet.Bind(this.progressSlider).For(v => v.value).To(vm => vm.Value).OneWay();

            bindingSet.Build();
        }
    }


    IEnumerator Unzip(ProgressBarViewModel progressBar) {
        progressBar.Tip = "Unziping";
        progressBar.Enabled = true;//Display the progress bar

        for(int i=0;i<30;i++) {            
            //TODO:Add unzip code here.

            progressBar.Value = (i/(float)30);            
            yield return null;
        }

        progressBar.Enabled = false;//Hide the progress bar
        progressBar.Tip = "";        
    }


## Tutorials and Examples

 ![](docs/images/Launcher.gif)

 ![](docs/images/Databinding.gif)

 ![](docs/images/ListView.gif)

 ![](docs/images/Localization.gif)

![](docs/images/Interaction.gif)

## Introduction
- Window View

  ![](docs/images/Window.png)
- Localization

  ![](docs/images/Localization.png)
- Databinding

  ![](docs/images/Databinding.png)
- Variable Example

  ![](docs/images/Variable.png)
- ListView Binding

  ![](docs/images/ListView.png)

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 15034148

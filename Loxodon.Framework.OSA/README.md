![](docs/images/icon.png)

# Loxodon Framework OSA

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)

[(中文版)](README_CN.md)

*Developed by Clark*

This plugin is developed based on [Optimized ScrollView Adapter](https://assetstore.unity.com/packages/tools/gui/optimized-scrollview-adapter-68436), adding data binding features to it, making it compatible with the LoxodonFramework.

## Requires ##

[Loxodon Framework](https://github.com/vovgou/loxodon-framework)

[Optimized ScrollView Adapter](https://assetstore.unity.com/packages/tools/gui/optimized-scrollview-adapter-68436)

**Note: This project is a plugin for Loxodon.Framework, and must be used in the Loxodon.Framework environment. Please install Loxodon.Framework and Optimized ScrollView Adapter before installing and using it.**

## Installation

### Install via *.unitypackage file

Download the Loxodon.Framework.OSA.unitypackage and import it into your project.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## Quick Start ##

    public class ListViewExampleViewModel : ViewModelBase {
        private int id = 0;
        private ObservableList<ItemViewModel> items;

        public ObservableList<ItemViewModel> Items {
            get { return this.items; }
            set { this.Set(ref items, value); }
        }

        public ListViewExampleViewModel() {
            this.CreateItems(3);
        }

        public void AddItem() {
            items.Add(CreateItem());
        }

        public void ChangeItem() {
            if (items != null && items.Count > 0) {
                var model = items[0];
                model.Color = DemosUtil.GetRandomColor();
            }
        }

        public void MoveItem() {
            if (items != null && items.Count > 1) {
                items.Move(0, items.Count - 1);
            }
        }

        public void ResetItem() {
            items.Clear();
        }

        private void CreateItems(int count) {
            this.items = new ObservableList<ItemViewModel>();
            for (int i = 0; i < count; i++)
                items.Add(CreateItem());
        }

        private ItemViewModel CreateItem() {
            return new ItemViewModel(this.items) {
                Title = "Item #" + (id++),
            };
        }
    }

    public class ListViewExample : MonoBehaviour {
        public Button addButton;
        public Button changeButton;
        public Button moveButton;
        public Button resetButton;
        public ListViewBindingAdapter listView;

        protected void Awake() {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }

        private void Start() {
            var bindingSet = this.CreateBindingSet<ListViewExample, ListViewExampleViewModel>();

            bindingSet.Bind(addButton).For(v => v.onClick).To(vm => vm.AddItem);
            bindingSet.Bind(changeButton).For(v => v.onClick).To(vm => vm.ChangeItem);
            bindingSet.Bind(moveButton).For(v => v.onClick).To(vm => vm.MoveItem);
            bindingSet.Bind(resetButton).For(v => v.onClick).To(vm => vm.ResetItem);
            bindingSet.Bind(listView).For(v => v.Items).To(vm => vm.Items);
            bindingSet.Build();

            this.SetDataContext(new ListViewExampleViewModel());
        }
    }

 ![](docs/images/list.gif)

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)

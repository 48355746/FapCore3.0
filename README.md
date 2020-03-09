
## Fap简介
它一个企业级应用的快速开发平台。包含了所有企业级应用开发所需要的所有基础模块。可以使您更关注于业务的开发。
[github](https://github.com/48355746/FapCore3.0)
## 基础模块
主要包含的基础模块有：用户管理，角色管理，菜单管理，权限管理，组织管理，员工管理，流程引擎，报表引擎，任务调度，字典管理，多语言管理，元数据管理等。
## 公共组件

### 1、表格

View Tag
```yaml
 <fap-grid id="faptable" grid-model="Model" auto-width="true"></fap-grid>
```
C#代码
```csharp
        public IActionResult TestGrid()
        {
            var model = this.GetJqGridModel("BonusTest");
            return View(model);
        }
```
![自动生成表格](https://img2020.cnblogs.com/blog/1852668/202003/1852668-20200309125612020-1464387791.png)
表格基础操作：新增，编辑，批量编辑，删除，查看，搜索，刷新，导出Excel，导入数据，打印等。
表格列基础功能：排序，筛选，分类，冻结等。
### 2、表单

```yaml
 <fap-form id="@Model.FormId"   form-model="Model"></fap-form>
```

```csharp
  FormViewModel fd = this.GetFormViewModel(menuColumn.TableName, menuColumn.GridId, fid, qs =>
                        {
                            qs.QueryCols = menuColumn.GridColumn;
                        });
                         return View(fd);
```
![表单](https://img2020.cnblogs.com/blog/1852668/202003/1852668-20200309125622256-2012512280.png)
表单包括的控件：文本框，日期，时间，参照，下拉框，数字，附件，多语言，复选框，多选列表，数值范围，星，富文本，多行文本等。
以上控件在元数据配置完毕自动生成。
### 3、树

```yaml
<fap-tree id="usergroup" is-async="true" get-url="/System/Api/Manage/UserGroup" edit-url="/System/Api/Manage/UserGroup"></fap-tree>
```
### 4、按钮

```yaml
<fap-button id="btnResetPassword" btn-tag="link" content="重置密码" icon-before="fa fa-cog blue" class-name="info"></fap-button>
```
### 5、多语言

```yaml
<fap-multilang lang-key="user" default-content="用户"></fap-multilang>
```

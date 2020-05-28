
## FapCore30介绍
基于.netcore30框架,面向企业级应用开发。
包含以下基础模块
- 数据字典
- 多语言
- 元数据
- 用户管理
- 角色管理
- 菜单管理
- 权限管理
- 组织管理
- 员工管理
- 流程引擎
- 报表引擎
- 任务调度
## 搭建环境
开发工具vs2019，.netcore30框架，支持mssql，mysql数据库。
1. clone代码到本地。
```shell
clone https://github.com/48355746/FapCore3.0.git
```
2. 新建一个数据库
```sql
CREATE DATABASE FapCore30 COLLATE Chinese_PRC_CI_AS 
```
3. 解压根目录下的MSSQL.zip文件。
4. sqlcmd执行MSSQL.sql文件。实例：
```sql
sqlcmd -S localhost -U sa -P 123 -d FapCore30 -i /var/opt/mssql/MSSQL.sql
```
5. 修改Fap.Hcm.Web项目下appsettings.json文件，配置数据库。
```json
"ConnectionString": {
    "sqlconnection": "Data Source=172.17.0.4;Initial Catalog=FapCore30;User ID=sa;Password=123;Enlist=false;Max Pool SIZE=500;Min Pool SIZE=50;MultipleActiveResultSets=True"
    "sqlconnection_slaver_1": "Data Source=172.17.0.5;Initial Catalog=FapCore30;User ID=sa;Password=123;Enlist=false;Max Pool SIZE=500;Min Pool SIZE=50;MultipleActiveResultSets=True",
    "sqlconnection_slaver_2": "Data Source=172.17.0.5;Initial Catalog=FapCore30;User ID=sa;Password=123;Enlist=false;Max Pool SIZE=500;Min Pool SIZE=50;MultipleActiveResultSets=True"
  }
```
mysql数据库请使用 mysqlconnection 名称。_slaver_1，_slaver_2为从数据库。没有可以不配置。
6.  启动 Fap.Hcm.Web。
## docker部署
根目录已包含Dockerfile文件
-  构造镜像
```shell
sudo docker build -t fapcore/hcm .
```
-  运行容器
数据卷如下：-v 时区,-v logs,-v 附件。说明：设置appsetting.json日志路径为LogPath（/var/fapcore/logs），设置附件路径为（/var/fapcore/annex）
```shell
sudo docker run --name fapcorehcm -d -p 5000:80 -p 5001:443 -v /etc/localtime:/etc/localtime -v /usr/docker/fapcorehcm/logs:/var/fapcore/logs -v /usr/docker/fapcorehcm/annex:/var/fapcore/annex fapcore/hcm
```
## 公共组件

### 1、表格组件
功能：增删改查，批量编辑，导出，导入excel，导出word，自定义统计图表。支持排列，排序，筛选，分类，冻结

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

### 2、表单
控件：文本框，日期，时间，参照，下拉框，数字，附件，多语言，复选框，多选列表，数值范围，星，富文本，多行文本等。
支持字段分组，校验。

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


### 3、按钮
自动绑定权限
```yaml
<fap-button id="btnResetPassword" btn-tag="link" content="重置密码" icon-before="fa fa-cog blue" class-name="info"></fap-button>
```
### 4、多语言标签
统一处理多语言
```yaml
<fap-multilang lang-key="user" default-content="用户"></fap-multilang>
```
## 人力资源系统
### 介绍
基于fapcore30平台。包括了人力资源系统几乎所有的模块（组织规划，人事管理，时间管理，薪资管理，保险管理，绩效管理，招聘管理，业务中心，统计报表，系统管理，员工自助，经理自助，总裁桌面等）。同时提供在线用户即时通讯功能。
### 演示地址
https://hrsoft.club


-- 删除表
drop table if exists  RptDataSet

GO
-- 创建表
CREATE TABLE `RptDataSet`( 
`Id`  INT(5)  AUTO_INCREMENT NOT NULL  comment '主键' ,
`Fid`  VARCHAR(20) NULL  comment '唯一标识' ,
`DataSetName`  VARCHAR(20)  comment '名称' ,
`Pid`  VARCHAR(20)  comment '父数据集' ,
`Content`  TEXT  comment '数据内容' ,
`IsDir`  TinyInt(5) NULL default 0  comment '是否为目录' ,
`OrgUid`  VARCHAR(20) NULL  comment '组织' ,
`GroupUid`  VARCHAR(20) NULL  comment '集团' ,
`EnableDate`  VARCHAR(19) NULL  comment '有效开始时间' ,
`DisableDate`  VARCHAR(19) NULL  comment '有效截止时间' ,
`Dr`  TinyInt(5) NULL default 0  comment '删除标记' ,
`Ts`  BIGINT(8) NULL default 0  comment '时间戳' ,
`CreateBy`  VARCHAR(20)  comment '创建人' ,
`CreateName`  VARCHAR(100)  comment '创建人名称' ,
`CreateDate`  VARCHAR(19) NULL  comment '创建时间' ,
`UpdateBy`  VARCHAR(20)  comment '更新人' ,
`UpdateName`  VARCHAR(100)  comment '更新人名称' ,
`UpdateDate`  VARCHAR(19) NULL  comment '更新时间' ,
PRIMARY KEY (Id))comment='报表数据集' ENGINE = InnoDB DEFAULT CHARSET = utf8mb4


GO

-- 删除表
drop table if exists  RptTemplate

GO
-- 创建表
CREATE TABLE `RptTemplate`( 
`Id`  INT(5)  AUTO_INCREMENT NOT NULL  comment '主键' ,
`Fid`  VARCHAR(20) NULL  comment '唯一标识' ,
`Pid`  VARCHAR(20)  comment '父报表' ,
`ReportName`  VARCHAR(50)  comment '报表名称' ,
`DataXml`  TEXT  comment '报表XML' ,
`IsDir`  TinyInt(5) NULL default 0  comment '是否目录' ,
`OrgUid`  VARCHAR(20) NULL  comment '组织' ,
`GroupUid`  VARCHAR(20) NULL  comment '集团' ,
`EnableDate`  VARCHAR(19) NULL  comment '有效开始时间' ,
`DisableDate`  VARCHAR(19) NULL  comment '有效截止时间' ,
`Dr`  TinyInt(5) NULL default 0  comment '删除标记' ,
`Ts`  BIGINT(8) NULL default 0  comment '时间戳' ,
`CreateBy`  VARCHAR(20)  comment '创建人' ,
`CreateName`  VARCHAR(100)  comment '创建人名称' ,
`CreateDate`  VARCHAR(19) NULL  comment '创建时间' ,
`UpdateBy`  VARCHAR(20)  comment '更新人' ,
`UpdateName`  VARCHAR(100)  comment '更新人名称' ,
`UpdateDate`  VARCHAR(19) NULL  comment '更新时间' ,
PRIMARY KEY (Id))comment='报表模板' ENGINE = InnoDB DEFAULT CHARSET = utf8mb4


GO

-- 删除表
drop table if exists  RptSimpleTemplate

GO
-- 创建表
CREATE TABLE `RptSimpleTemplate`( 
`Id`  INT(5)  AUTO_INCREMENT NOT NULL  comment '主键' ,
`Fid`  VARCHAR(20) NULL  comment '唯一标识' ,
`Pid`  VARCHAR(20)  comment '父报表' ,
`ReportName`  VARCHAR(100)  comment '报表名称' ,
`DataXml`  TEXT  comment '报表XML' ,
`XlsFile`  VARCHAR(20) NULL  comment 'Excel文件' ,
`IsDir`  TinyInt(5) NULL default 0  comment '是否目录' ,
`OrgUid`  VARCHAR(20) NULL  comment '组织' ,
`GroupUid`  VARCHAR(20) NULL  comment '集团' ,
`EnableDate`  VARCHAR(19) NULL  comment '有效开始时间' ,
`DisableDate`  VARCHAR(19) NULL  comment '有效截止时间' ,
`Dr`  TinyInt(5) NULL default 0  comment '删除标记' ,
`Ts`  BIGINT(8) NULL default 0  comment '时间戳' ,
`CreateBy`  VARCHAR(20)  comment '创建人' ,
`CreateName`  VARCHAR(100)  comment '创建人名称' ,
`CreateDate`  VARCHAR(19) NULL  comment '创建时间' ,
`UpdateBy`  VARCHAR(20)  comment '更新人' ,
`UpdateName`  VARCHAR(100)  comment '更新人名称' ,
`UpdateDate`  VARCHAR(19) NULL  comment '更新时间' ,
PRIMARY KEY (Id))comment='简单报表模板' ENGINE = InnoDB DEFAULT CHARSET = utf8mb4


GO

--FapTable元数据
INSERT INTO `FapTable`(`Fid`,`TableName`,`TableComment`,`TableType`,`TableCategory`,`TableMode`,`SubTable`,`MainTable`,`IsTree`,`IsPagination`,`IsSync`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`TableFeature`,`DataInterceptor`,`TraceAble`,`TableDesc`,`ScriptInjection`,`SqlInjection`,`LangZhTW`,`LangEn`,`LangJa`,`ProductUid`) VALUES('dfb911e5ad4524ed0102','RptDataSet','报表数据集','SYSTEM','Rpt','SINGLE',NULL,'',0,0,1,NULL,NULL,'2016-03-01 22:23:19','9999-12-31 23:59:59',0,635924677996134035,NULL,NULL,'2016-03-01 22:23:19',NULL,NULL,'2016-03-01 22:23:19','',NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'FAP')
GO
--FapColumn元数据
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0107','RptDataSet','Content','数据内容',NULL,NULL,'CLOB','1',20,100,4,0,1,1,1,0,'MEMO',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Data content',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010e','RptDataSet','CreateBy','创建人',NULL,NULL,'STRING','1',20,10,986,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Founder',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0110','RptDataSet','CreateDate','创建时间',NULL,NULL,'DATETIME','1',19,10,988,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010f','RptDataSet','CreateName','创建人名称',NULL,NULL,'STRING','1',100,10,987,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created Name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0105','RptDataSet','DataSetName','名称',NULL,NULL,'STRING','1',20,80,2,0,1,1,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010b','RptDataSet','DisableDate','有效截止时间',NULL,NULL,'DATETIME','1',19,19,983,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Expiration Time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010c','RptDataSet','Dr','删除标记',NULL,NULL,'BOOL','1',10,10,984,0,0,0,0,1,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Remove mark',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010a','RptDataSet','EnableDate','有效开始时间',NULL,NULL,'DATETIME','1',19,19,982,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Valid start time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0104','RptDataSet','Fid','唯一标识','${FAP::UUID}',NULL,'UID','1',20,20,1,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Uniquely identifies',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0109','RptDataSet','GroupUid','集团','${FAP::UUID}',NULL,'UID','1',10,10,981,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'group',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0103','RptDataSet','Id','主键',NULL,NULL,'PK','0',10,10,0,0,0,0,0,1,'INT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Primary key',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('ec2d11e5841926846526','RptDataSet','IsDir','是否为目录',NULL,NULL,'BOOL','1',20,100,5,0,1,1,1,0,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-17 18:43:57',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Whether the directory',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0108','RptDataSet','OrgUid','组织','${FAP::UUID}',NULL,'UID','1',10,10,980,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'organization',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0106','RptDataSet','Pid','父数据集',NULL,NULL,'STRING','1',20,100,3,0,1,1,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Parent dataset',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed010d','RptDataSet','Ts','时间戳',NULL,NULL,'LONG','1',10,10,985,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Time stamp',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0111','RptDataSet','UpdateBy','更新人',NULL,NULL,'STRING','1',20,10,989,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updater',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0113','RptDataSet','UpdateDate','更新时间',NULL,NULL,'DATETIME','1',19,10,991,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('dfb911e5ad4524ed0112','RptDataSet','UpdateName','更新人名称',NULL,NULL,'STRING','1',100,10,990,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-03-17 18:43:57','9999-12-31 23:59:59',0,635938370378572471,NULL,NULL,'2016-03-17 18:43:57',NULL,NULL,'2016-03-01 22:23:19',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated Title',NULL,NULL,NULL,NULL,NULL)
GO

INSERT INTO `RptDataSet`(`Fid`,`DataSetName`,`Pid`,`Content`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`IsDir`) VALUES('f74711e5acb7ffc3659f','数据集','0',NULL,NULL,NULL,'2016-03-31 21:53:52','9999-12-31 23:59:59',0,635950580320602463,NULL,NULL,'2016-03-31 21:53:52',NULL,NULL,'2016-03-31 21:53:52',1)
GO
INSERT INTO `RptDataSet`(`Fid`,`DataSetName`,`Pid`,`Content`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`IsDir`) VALUES('f74811e5acb76925c59c','人员统计','f74711e5acb7ffc3659f',NULL,NULL,NULL,'2016-03-31 21:56:48','9999-12-31 23:59:59',0,635983402556931110,NULL,NULL,'2016-03-31 21:56:48',NULL,NULL,'2016-05-08 21:37:35',1)
GO
INSERT INTO `RptDataSet`(`Fid`,`DataSetName`,`Pid`,`Content`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`IsDir`) VALUES('f74811e5acb794abf1db','统计性别','f74811e5acb76925c59c',NULL,NULL,NULL,'2016-03-31 21:58:01','9999-12-31 23:59:59',0,635983403081541116,NULL,NULL,'2016-03-31 21:58:01',NULL,NULL,'2016-05-08 21:38:28',1)
GO
INSERT INTO `RptDataSet`(`Fid`,`DataSetName`,`Pid`,`Content`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`IsDir`) VALUES('151a11e6a3869e3f4fd8','按部门统计性别人数','f74811e5acb794abf1db','<?xml version="1.0" encoding="utf-16"?>
<RptDataSetEntity xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Fid>151a11e6a3869e3f4fd8</Fid>
  <Name>按部门统计性别人数</Name>
  <SQL>SELECT DeptUid AS 部门ID,(SELECT b0.DeptName FROM OrgDept b0 WHERE b0.Fid=DeptUid AND (b0.EnableDate&lt;=@CurrentDate AND b0.DisableDate&gt;@CurrentDate AND b0.Dr=0) ) AS 部门名称,(SELECT b0.DeptCode FROM OrgDept b0 WHERE b0.Fid=DeptUid AND (b0.EnableDate&lt;=@CurrentDate AND b0.DisableDate&gt;@CurrentDate AND b0.Dr=0) ) AS 部门编号,SUM(CASE WHEN Gender=''male'' THEN 1 ELSE 0 END) AS 男,SUM(CASE WHEN Gender=''female'' THEN 1 ELSE 0 END) AS 女,SUM(CASE WHEN Gender=''unknown'' THEN 1 ELSE 0 END) AS 未知 FROM Employee GROUP BY DeptUid ORDER BY DeptUid ASC</SQL>
  <OutputFields>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a6f</Fid>
      <TableName />
      <FieldName>DeptUid</FieldName>
      <FieldAlias>部门ID</FieldAlias>
    </RptDataField>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a70</Fid>
      <TableName />
      <FieldName>(Select b0.DeptName
From OrgDept b0
Where b0.Fid = DeptUid and (b0.EnableDate &lt;= @CurrentDate and b0.DisableDate &gt; @CurrentDate and b0.Dr = 0))</FieldName>
      <FieldAlias>部门名称</FieldAlias>
    </RptDataField>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a71</Fid>
      <TableName />
      <FieldName>(Select b0.DeptCode
From OrgDept b0
Where b0.Fid = DeptUid and (b0.EnableDate &lt;= @CurrentDate and b0.DisableDate &gt; @CurrentDate and b0.Dr = 0))</FieldName>
      <FieldAlias>部门编号</FieldAlias>
    </RptDataField>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a72</Fid>
      <TableName />
      <FieldName>SUM(CASE 
  WHEN Gender = ''male'' THEN 1
  ELSE 0
END)</FieldName>
      <FieldAlias>男</FieldAlias>
    </RptDataField>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a73</Fid>
      <TableName />
      <FieldName>SUM(CASE 
  WHEN Gender = ''female'' THEN 1
  ELSE 0
END)</FieldName>
      <FieldAlias>女</FieldAlias>
    </RptDataField>
    <RptDataField>
      <Fid>1a9911e6aba3f8be1a74</Fid>
      <TableName />
      <FieldName>SUM(CASE 
  WHEN Gender = ''unknown'' THEN 1
  ELSE 0
END)</FieldName>
      <FieldAlias>未知</FieldAlias>
    </RptDataField>
  </OutputFields>
  <InputParams />
</RptDataSetEntity>',NULL,NULL,'2016-05-08 20:44:36','9999-12-31 23:59:59',0,635989415505297374,NULL,NULL,'2016-05-08 20:44:36',NULL,NULL,'2016-05-15 20:39:10',0)
GO

--FapTable元数据
INSERT INTO `FapTable`(`Fid`,`TableName`,`TableComment`,`TableType`,`TableCategory`,`TableMode`,`SubTable`,`MainTable`,`IsTree`,`IsPagination`,`IsSync`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`TableFeature`,`DataInterceptor`,`TraceAble`,`TableDesc`,`ScriptInjection`,`SqlInjection`,`LangZhTW`,`LangEn`,`LangJa`,`ProductUid`) VALUES('d39c11e5abc5eafe649f','RptTemplate','报表模板','SYSTEM','Rpt','SINGLE',NULL,'',0,0,1,NULL,NULL,'2016-02-15 12:31:02','9999-12-31 23:59:59',0,635911362625587520,NULL,NULL,'2016-02-15 12:31:02',NULL,NULL,'2016-02-15 12:31:02','',NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'FAP')
GO
--FapColumn元数据
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64aa','RptTemplate','CreateBy','创建人',NULL,NULL,'STRING','1',20,10,986,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'founder',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64ac','RptTemplate','CreateDate','创建时间',NULL,NULL,'DATETIME','1',19,10,988,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64ab','RptTemplate','CreateName','创建人名称',NULL,NULL,'STRING','1',100,10,987,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created Name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d3a111e5abc59e7e2baf','RptTemplate','DataXml','报表XML',NULL,NULL,'CLOB','1',20,100,4,0,1,0,0,0,'MEMO',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 13:04:41',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Report XML',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a7','RptTemplate','DisableDate','有效截止时间',NULL,NULL,'DATETIME','1',19,19,983,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Expiration Time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a8','RptTemplate','Dr','删除标记',NULL,NULL,'BOOL','1',10,10,984,0,0,0,0,1,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Remove mark',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a6','RptTemplate','EnableDate','有效开始时间',NULL,NULL,'DATETIME','1',19,19,982,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Valid start time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a1','RptTemplate','Fid','唯一标识','${FAP::UUID}',NULL,'UID','1',20,20,1,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Uniquely identifies',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a5','RptTemplate','GroupUid','集团','${FAP::UUID}',NULL,'UID','1',10,10,981,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'group',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a0','RptTemplate','Id','主键',NULL,NULL,'PK','0',10,10,0,0,0,0,0,1,'INT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Primary key',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('f75911e5acb7d9c97214','RptTemplate','IsDir','是否目录',NULL,NULL,'BOOL','1',20,100,5,0,1,1,1,0,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-04-01 00:01:39',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Whether the directory',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a4','RptTemplate','OrgUid','组织','${FAP::UUID}',NULL,'UID','1',10,10,980,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'organization',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a2','RptTemplate','Pid','父报表',NULL,NULL,'STRING','1',20,80,2,0,1,0,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Parent Report',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a3','RptTemplate','ReportName','报表名称',NULL,NULL,'STRING','1',50,100,3,0,1,1,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Report Name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64a9','RptTemplate','Ts','时间戳',NULL,NULL,'LONG','1',10,10,985,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Timestamp',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64ad','RptTemplate','UpdateBy','更新人',NULL,NULL,'STRING','1',20,10,989,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'updater',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64af','RptTemplate','UpdateDate','更新时间',NULL,NULL,'DATETIME','1',19,10,991,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('d39c11e5abc5eafe64ae','RptTemplate','UpdateName','更新人名称',NULL,NULL,'STRING','1',100,10,990,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-01 00:01:39','9999-12-31 23:59:59',0,635950656992775988,NULL,NULL,'2016-04-01 00:01:39',NULL,NULL,'2016-02-15 12:31:02',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated Title',NULL,NULL,NULL,NULL,NULL)
GO

INSERT INTO `RptTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`IsDir`) VALUES('f74711e5acb7ffc3659f','0','根目录',NULL,NULL,NULL,'2016-03-31 21:53:52','9999-12-31 23:59:59',0,635950580320602463,NULL,NULL,NULL,NULL,NULL,NULL,NULL)
GO

--FapTable元数据
INSERT INTO `FapTable`(`Fid`,`TableName`,`TableComment`,`TableType`,`TableCategory`,`TableMode`,`SubTable`,`MainTable`,`IsTree`,`IsPagination`,`IsSync`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`TableFeature`,`DataInterceptor`,`TraceAble`,`TableDesc`,`ScriptInjection`,`SqlInjection`,`LangZhTW`,`LangEn`,`LangJa`,`ProductUid`) VALUES('017811e683d8eb655ef7','RptSimpleTemplate','简单报表模板','SYSTEM','Rpt','SINGLE',NULL,'',0,0,1,NULL,NULL,'2016-04-13 21:09:14','9999-12-31 23:59:59',0,635961785548371319,NULL,NULL,'2016-04-13 21:09:14',NULL,NULL,'2016-04-13 21:09:14','',NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'FAP')
GO
--FapColumn元数据
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f05','RptSimpleTemplate','CreateBy','创建人',NULL,NULL,'STRING','1',20,10,986,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'founder',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f07','RptSimpleTemplate','CreateDate','创建时间',NULL,NULL,'DATETIME','1',20,10,988,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f06','RptSimpleTemplate','CreateName','创建人名称',NULL,NULL,'STRING','1',100,10,987,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Created Name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655efc','RptSimpleTemplate','DataXml','报表XML',NULL,NULL,'CLOB','1',20,100,4,0,1,1,1,0,'MEMO',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Report XML',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f02','RptSimpleTemplate','DisableDate','有效截止时间',NULL,NULL,'DATETIME','1',20,19,983,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Expiration Time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f03','RptSimpleTemplate','Dr','删除标记',NULL,NULL,'BOOL','1',10,10,984,0,0,0,0,1,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Remove mark',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f01','RptSimpleTemplate','EnableDate','有效开始时间',NULL,NULL,'DATETIME','1',20,19,982,0,0,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Valid start time',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655ef9','RptSimpleTemplate','Fid','唯一标识','${FAP::UUID}',NULL,'UID','1',20,20,1,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Uniquely identifies',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f00','RptSimpleTemplate','GroupUid','集团','${FAP::UUID}',NULL,'UID','1',20,10,981,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'group',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655ef8','RptSimpleTemplate','Id','主键',NULL,NULL,'PK','0',20,10,0,0,0,0,0,1,'INT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Primary key',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655efe','RptSimpleTemplate','IsDir','是否目录',NULL,NULL,'BOOL','1',20,100,6,0,1,1,1,0,'CHECKBOX',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Whether the directory',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655eff','RptSimpleTemplate','OrgUid','组织','${FAP::UUID}',NULL,'UID','1',20,10,980,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'organization',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655efa','RptSimpleTemplate','Pid','父报表',NULL,NULL,'STRING','1',20,80,2,0,1,1,1,0,'REFERENCE',NULL,'RptSimpleTemplate','Fid',NULL,'ReportName',NULL,NULL,NULL,'GridReference',NULL,NULL,NULL,'',NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Parent Report',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655efb','RptSimpleTemplate','ReportName','报表名称',NULL,NULL,'STRING','1',100,100,3,0,1,1,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Report Name',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f04','RptSimpleTemplate','Ts','时间戳',NULL,NULL,'LONG','1',20,10,985,0,0,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Timestamp',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f08','RptSimpleTemplate','UpdateBy','更新人',NULL,NULL,'STRING','1',20,10,989,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'updater',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f0a','RptSimpleTemplate','UpdateDate','更新时间',NULL,NULL,'DATETIME','1',20,10,991,0,1,0,0,1,'DATETIME',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655f09','RptSimpleTemplate','UpdateName','更新人名称',NULL,NULL,'STRING','1',100,10,990,0,1,0,0,1,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,0,0,NULL,0,0,0,NULL,NULL,NULL,'Updated Title',NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `FapColumn`(`Fid`,`TableName`,`ColName`,`ColComment`,`ColDefault`,`DefaultValueClass`,`ColType`,`ColProperty`,`ColLength`,`DisplayWidth`,`ColOrder`,`ColPrecision`,`NullAble`,`ShowAble`,`EditAble`,`IsDefaultCol`,`CtrlType`,`DisplayFormat`,`RefTable`,`RefID`,`RefCode`,`RefName`,`RefCondition`,`RefCols`,`ComboxSource`,`RefType`,`MainTable`,`MainTableCol`,`CalculationExpr`,`ColGroup`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`,`FileSuffix`,`FileCount`,`FileSize`,`ComponentUid`,`IsObsolete`,`IsMultiLang`,`MultiAble`,`RemoteChkURL`,`RemoteChkMsg`,`LangZhTW`,`LangEn`,`LangJa`,`SortDirection`,`MaxValue`,`MinValue`,`RefReturnMapping`) VALUES('017811e683d8eb655efd','RptSimpleTemplate','XlsFile','Excel文件','${FAP::UUID}',NULL,'UID','1',20,100,5,0,1,1,1,0,'TEXT',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,'2016-04-16 13:47:37','9999-12-31 23:59:59',0,635964112572029739,NULL,NULL,'2016-04-16 13:47:37',NULL,NULL,'2016-04-13 21:09:14',NULL,10,10240,NULL,0,0,0,NULL,NULL,NULL,'Excel files',NULL,NULL,NULL,NULL,NULL)
GO

INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('f74711e5acb7ffc3659f','0','报表模板',NULL,NULL,1,NULL,NULL,'2016-03-31 21:53:52','9999-12-31 23:59:59',0,635950580320602463,NULL,NULL,NULL,NULL,NULL,NULL)
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('0bc211e6ac675086fc7a','f74711e5acb7ffc3659f','员工履历',NULL,'0bc211e6ac675086fc7b',1,NULL,NULL,'2016-04-26 23:19:49','9999-12-31 23:59:59',0,635973095894463515,NULL,NULL,'2016-04-26 23:19:49',NULL,NULL,'2016-04-26 23:19:49')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('0bc311e6ac6759db83dc','0bc211e6ac675086fc7a','员工履历表','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>0bc311e6ac6759db83dc</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.年龄</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Age</FieldName>
      <FieldAlias>员工信息_年龄</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.婚姻</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Marriage</FieldName>
      <FieldAlias>员工信息_婚姻</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.籍贯</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>NativePlace</FieldName>
      <FieldAlias>员工信息_籍贯</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.民族</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Nation</FieldName>
      <FieldAlias>员工信息_民族</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.政治面貌</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Political</FieldName>
      <FieldAlias>员工信息_政治面貌</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.学历</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Education</FieldName>
      <FieldAlias>员工信息_学历</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.聘任职称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmploymentTitle</FieldName>
      <FieldAlias>员工信息_聘任职称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.身份证号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IDCard</FieldName>
      <FieldAlias>员工信息_身份证号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.手机</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Mobile</FieldName>
      <FieldAlias>员工信息_手机</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.邮编</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ZipCode</FieldName>
      <FieldAlias>员工信息_邮编</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Emergency</FieldName>
      <FieldAlias>员工信息_紧急联系人</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人电话</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmergencyPhone</FieldName>
      <FieldAlias>员工信息_紧急联系人电话</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.职务</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpJob</FieldName>
      <FieldAlias>员工信息_职务</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.照片</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpPhoto</FieldName>
      <FieldAlias>员工信息_照片</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>IMAGE</FormatType>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.开始日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>StartDate</FieldName>
      <FieldAlias>员工学历子集_开始日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date1</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.结束日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EndDate</FieldName>
      <FieldAlias>员工学历子集_结束日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date1</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.专业</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Major</FieldName>
      <FieldAlias>员工学历子集_专业</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.学历</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Education</FieldName>
      <FieldAlias>员工学历子集_学历</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址2</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址2</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>学历学位.毕业院校</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>University</FieldName>
      <FieldAlias>学历学位_毕业院校</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.开始日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>StartDate</FieldName>
      <FieldAlias>工作经历_开始日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.结束日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EndDate</FieldName>
      <FieldAlias>工作经历_结束日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.工作单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkUnit</FieldName>
      <FieldAlias>工作经历_工作单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.成员姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>MemberName</FieldName>
      <FieldAlias>家庭成员_成员姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.与本人关系</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Relations</FieldName>
      <FieldAlias>家庭成员_与本人关系</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.出生日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DateOfBirth</FieldName>
      <FieldAlias>家庭成员_出生日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date3</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>家庭成员.工作单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkUnit</FieldName>
      <FieldAlias>家庭成员_工作单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.工作职务</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkJob</FieldName>
      <FieldAlias>家庭成员_工作职务</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称系列</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TitleSeries</FieldName>
      <FieldAlias>职称_职称系列</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称专业</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Professional</FieldName>
      <FieldAlias>职称_职称专业</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称等级1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>GradeTitle</FieldName>
      <FieldAlias>职称_职称等级1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.获取时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IssueDate</FieldName>
      <FieldAlias>职称_获取时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.有效时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ValideDate</FieldName>
      <FieldAlias>职称_有效时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.职务/职级</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkJob</FieldName>
      <FieldAlias>工作经历_职务/职级</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.证书名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Name</FieldName>
      <FieldAlias>资格证书_证书名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.专业名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ProfessionalName</FieldName>
      <FieldAlias>资格证书_专业名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.证书编号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>CertificateNumber</FieldName>
      <FieldAlias>资格证书_证书编号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.获取证书时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IssueDate</FieldName>
      <FieldAlias>资格证书_获取证书时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.注册单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>RegisterUnit</FieldName>
      <FieldAlias>资格证书_注册单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.Fid = @员工</Condition>
      <ConditionParams>
        <string>员工编码</string>
      </ConditionParams>
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>Fid</DataSourceField>
            <DataSourceFieldAlias>唯一标识</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@员工|REFERENCE|Employee|Fid|EmpName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceAlias>员工学历子集</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubEducation</SubTable>
        <SubTableAlias>员工学历子集</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceAlias>工作经历</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubCareer</SubTable>
        <SubTableAlias>工作经历</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceAlias>家庭成员</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubFamily</SubTable>
        <SubTableAlias>家庭成员</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceAlias>职称</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubTitle</SubTable>
        <SubTableAlias>职称</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceAlias>资格证书</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>SubTable</TableType>
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','0bc311e6ac6759db83dd',0,NULL,NULL,'2016-04-26 23:27:14','9999-12-31 23:59:59',0,635988528324852818,NULL,NULL,'2016-04-26 23:27:14',NULL,NULL,'2016-05-14 20:00:32')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('0db111e69e31dc177796','0bc211e6ac675086fc7a','员工信息列表','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>0db111e69e31dc177796</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.员工编码</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCode</FieldName>
      <FieldAlias>员工信息_员工编码</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.年龄</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Age</FieldName>
      <FieldAlias>员工信息_年龄</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.出生日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DateOfBirth</FieldName>
      <FieldAlias>员工信息_出生日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.身份证号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IDCard</FieldName>
      <FieldAlias>员工信息_身份证号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.员工类别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCategory</FieldName>
      <FieldAlias>员工信息_员工类别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <DataSourceFieldOrder>
        <FieldName>EmpCode</FieldName>
        <FieldOrder>Desc</FieldOrder>
      </DataSourceFieldOrder>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
  <SqlCells />
</RptTemplate>','0db111e69e31dc177797',0,NULL,NULL,'2016-11-01 13:45:50','9999-12-31 23:59:59',0,636136047515233129,NULL,NULL,'2016-04-29 10:27:04',NULL,NULL,'2016-11-01 13:45:51')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('114211e6b644b61d371e','f74711e5acb7ffc3659f','员工薪资',NULL,'114211e6b644b61d371f',1,NULL,NULL,'2016-05-03 23:21:31','9999-12-31 23:59:59',0,635979144912950550,NULL,NULL,'2016-05-03 23:21:31',NULL,NULL,'2016-05-03 23:21:31')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('114211e6b644c5989b77','114211e6b644b61d371e','薪资发放表','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>114211e6b644c5989b77</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>薪资中心.员工</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpUid</FieldName>
      <FieldAlias>薪资中心_员工</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.员工类别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCategory</FieldName>
      <FieldAlias>薪资中心_员工类别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.工资年月</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>PayYM</FieldName>
      <FieldAlias>薪资中心_工资年月</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.养老保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceA</FieldName>
      <FieldAlias>薪资中心_养老保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.养老金补缴</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceAB</FieldName>
      <FieldAlias>薪资中心_养老金补缴</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.失业保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceB</FieldName>
      <FieldAlias>薪资中心_失业保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.失业保险金补缴</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceBB</FieldName>
      <FieldAlias>薪资中心_失业保险金补缴</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.工伤保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceC</FieldName>
      <FieldAlias>薪资中心_工伤保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.工伤保险金补缴</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceCB</FieldName>
      <FieldAlias>薪资中心_工伤保险金补缴</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.生育保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceD</FieldName>
      <FieldAlias>薪资中心_生育保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.生育保险金补缴</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceDB</FieldName>
      <FieldAlias>薪资中心_生育保险金补缴</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.医疗保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceE</FieldName>
      <FieldAlias>薪资中心_医疗保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.医疗保险金补缴</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceEB</FieldName>
      <FieldAlias>薪资中心_医疗保险金补缴</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.补充医疗保险金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceF</FieldName>
      <FieldAlias>薪资中心_补充医疗保险金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.住房公积金</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>InsuranceG</FieldName>
      <FieldAlias>薪资中心_住房公积金</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.迟到扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeA</FieldName>
      <FieldAlias>薪资中心_迟到扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.事假扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeB</FieldName>
      <FieldAlias>薪资中心_事假扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.病假扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeC</FieldName>
      <FieldAlias>薪资中心_病假扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.旷工扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeD</FieldName>
      <FieldAlias>薪资中心_旷工扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.基本工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>BasePay</FieldName>
      <FieldAlias>薪资中心_基本工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.应发工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ShouldBePay</FieldName>
      <FieldAlias>薪资中心_应发工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.实发工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>RealPayroll</FieldName>
      <FieldAlias>薪资中心_实发工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceAlias>薪资中心</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition>PayCenter.PayYM = @工资年月 AND PayCenter.EmpCode = @员工编号</Condition>
      <ConditionParams>
        <string>员工编号</string>
        <string>工资年月</string>
      </ConditionParams>
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>PayCenter</DataSource>
            <DataSourceAlias>薪资中心</DataSourceAlias>
            <DataSourceField>PayYM</DataSourceField>
            <DataSourceFieldAlias>工资年月</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@工资年月|TEXT</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
          <RptConditionItem>
            <DataSource>PayCenter</DataSource>
            <DataSourceAlias>薪资中心</DataSourceAlias>
            <DataSourceField>EmpCode</DataSourceField>
            <DataSourceFieldAlias>员工编码</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@员工编号|REFERENCE|Employee|EmpCode|EmpName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','114211e6b644c5989b78',0,NULL,NULL,'2016-05-03 23:21:57','9999-12-31 23:59:59',0,636006187828180267,NULL,NULL,'2016-05-03 23:21:57',NULL,NULL,'2016-06-04 06:33:02')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('11cf11e69e31e98247d0','114211e6b644b61d371e','员工工资卡片','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>11cf11e69e31e98247d0</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>薪资中心.员工</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpUid</FieldName>
      <FieldAlias>薪资中心_员工</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>薪资中心.基本工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>BasePay</FieldName>
      <FieldAlias>薪资中心_基本工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.事假扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeB</FieldName>
      <FieldAlias>薪资中心_事假扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.病假扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeC</FieldName>
      <FieldAlias>薪资中心_病假扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.迟到扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeA</FieldName>
      <FieldAlias>薪资中心_迟到扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.旷工扣款</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TimeD</FieldName>
      <FieldAlias>薪资中心_旷工扣款</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.应发工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ShouldBePay</FieldName>
      <FieldAlias>薪资中心_应发工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>薪资中心.实发工资</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>RealPayroll</FieldName>
      <FieldAlias>薪资中心_实发工资</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Float</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>SUM(C{0}:F{0})</Data>
      <IsFormula>true</IsFormula>
      <Formula>=SUM(C3:F3)</Formula>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>PayCenter</DataSourceName>
      <DataSourceAlias>薪资中心</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Repeat</ShowType>
      <TableType>MainTable</TableType>
      <Condition>PayCenter.DeptUid = @部门</Condition>
      <ConditionParams>
        <string>部门Id</string>
      </ConditionParams>
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>PayCenter</DataSource>
            <DataSourceAlias>薪资中心</DataSourceAlias>
            <DataSourceField>DeptUid</DataSourceField>
            <DataSourceFieldAlias>部门</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@部门|REFERENCE|OrgDept|Fid|DeptName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','11cf11e69e31e98247d1',0,NULL,NULL,'2016-05-04 16:12:16','9999-12-31 23:59:59',0,636008162284630525,NULL,NULL,'2016-05-04 16:12:16',NULL,NULL,'2016-06-06 13:23:48')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('150611e6a38642a57465','f74711e5acb7ffc3659f','员工统计',NULL,'150611e6a38642a57466',1,NULL,NULL,'2016-05-08 18:18:52','9999-12-31 23:59:59',0,635983283324305382,NULL,NULL,'2016-05-08 18:18:52',NULL,NULL,'2016-05-08 18:18:52')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('150611e6a3866350cf5d','150611e6a38642a57465','按部门性别统计','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>150611e6a3866350cf5d</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>按部门统计性别人数.男</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceType>DataSet</DataSourceType>
      <FieldName>男</FieldName>
      <FieldAlias>按部门统计性别人数_男</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Int</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>按部门统计性别人数.女</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceType>DataSet</DataSourceType>
      <FieldName>女</FieldName>
      <FieldAlias>按部门统计性别人数_女</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Int</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>按部门统计性别人数.未知</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceType>DataSet</DataSourceType>
      <FieldName>未知</FieldName>
      <FieldAlias>按部门统计性别人数_未知</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>NUMBER</FormatType>
        <FormatCode>Int</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>SUM(B{0}:D{0})</Data>
      <IsFormula>true</IsFormula>
      <Formula>=SUM(B2:D2)</Formula>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>按部门统计性别人数.部门名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceType>DataSet</DataSourceType>
      <FieldName>部门名称</FieldName>
      <FieldAlias>按部门统计性别人数_部门名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>按部门统计性别人数</DataSourceName>
      <DataSourceAlias>按部门统计性别人数</DataSourceAlias>
      <DataSourceType>DataSet</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition>部门ID = @部门</Condition>
      <ConditionParams>
        <string>部门编号</string>
      </ConditionParams>
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>按部门统计性别人数</DataSource>
            <DataSourceAlias>按部门统计性别人数</DataSourceAlias>
            <DataSourceField>DeptUid</DataSourceField>
            <DataSourceFieldAlias>部门ID</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@部门|REFERENCE|OrgDept|Fid|DeptName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','150611e6a3866350cf5e',0,NULL,NULL,'2016-05-08 18:19:47','9999-12-31 23:59:59',0,636006175783711363,NULL,NULL,'2016-05-08 18:19:47',NULL,NULL,'2016-06-04 06:12:58')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('199c11e6a1a644bef110','f74711e5acb7ffc3659f','测试',NULL,'199c11e6a1a644bef111',1,NULL,NULL,'2016-05-14 14:22:45','9999-12-31 23:59:59',0,635988325650735633,NULL,NULL,'2016-05-14 14:22:45',NULL,NULL,'2016-05-14 14:22:45')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('199c11e6a1a64d52af3a','199c11e6a1a644bef110','001','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>199c11e6a1a64d52af3a</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <DataSourceFieldOrder>
        <FieldName>EmpName</FieldName>
        <FieldOrder>Asc</FieldOrder>
      </DataSourceFieldOrder>
      <Condition>Employee.EmpName LIKE @姓名</Condition>
      <ConditionParams />
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <IsBelongToSqlCell>false</IsBelongToSqlCell>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>EmpName</DataSourceField>
            <DataSourceFieldAlias>姓名</DataSourceFieldAlias>
            <Operator>LIKE</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@姓名|TEXT</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
  <SqlCells />
</RptTemplate>','199c11e6a1a64d52af3b',0,NULL,NULL,'2016-11-01 12:35:55','9999-12-31 23:59:59',0,636136005561183518,NULL,NULL,'2016-05-14 14:22:59',NULL,NULL,'2016-11-01 12:35:56')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('1a7211e6aba332f576f7','199c11e6a1a644bef110','002','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>1a7211e6aba332f576f7</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.出生日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DateOfBirth</FieldName>
      <FieldAlias>员工信息_出生日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.DateOfBirth &gt;= @出生日期</Condition>
      <ConditionParams />
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>DateOfBirth</DataSourceField>
            <DataSourceFieldAlias>出生日期</DataSourceFieldAlias>
            <Operator>GTE</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@出生日期|DATE</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','1a7211e6aba332f576f8',0,NULL,NULL,'2016-05-15 15:54:07','9999-12-31 23:59:59',0,635989249380584023,NULL,NULL,'2016-05-15 15:54:07',NULL,NULL,'2016-05-15 16:02:18')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('1a7311e6aba3ae78e8f6','199c11e6a1a644bef110','003','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>1a7311e6aba3ae78e8f6</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.政治面貌</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Political</FieldName>
      <FieldAlias>员工信息_政治面貌</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.Political = @政治面貌</Condition>
      <ConditionParams />
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>Political</DataSourceField>
            <DataSourceFieldAlias>政治面貌</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@政治面貌|DICT|EmpPolitical</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','1a7311e6aba3ae78e8f7',0,NULL,NULL,'2016-05-15 16:04:44','9999-12-31 23:59:59',0,635989250842099574,NULL,NULL,'2016-05-15 16:04:44',NULL,NULL,'2016-05-15 16:04:44')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('1a7311e6aba3d74439db','199c11e6a1a644bef110','004','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>1a7311e6aba3d74439db</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.年龄</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Age</FieldName>
      <FieldAlias>员工信息_年龄</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.Age &gt;= @年龄</Condition>
      <ConditionParams />
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>Age</DataSourceField>
            <DataSourceFieldAlias>年龄</DataSourceFieldAlias>
            <Operator>GTE</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@年龄|NUMBER</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','1a7311e6aba3d74439dc',0,NULL,NULL,'2016-05-15 16:05:52','9999-12-31 23:59:59',0,635989251526512731,NULL,NULL,'2016-05-15 16:05:52',NULL,NULL,'2016-05-15 16:05:52')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('2b9811e68819ec1a53bd','199c11e6a1a644bef110','测试_员工列表','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>2b9811e68819ec1a53bd</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.员工类别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCategory</FieldName>
      <FieldAlias>员工信息_员工类别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','2b9811e68819ec1a53be',0,NULL,NULL,'2016-06-06 11:44:08','9999-12-31 23:59:59',0,636008102488810525,NULL,NULL,'2016-06-06 11:44:08',NULL,NULL,'2016-06-06 11:44:08')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('2b9911e68819cecb510d','199c11e6a1a644bef110','测试_员工基本信息','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>2b9911e68819cecb510d</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.年龄</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Age</FieldName>
      <FieldAlias>员工信息_年龄</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.婚姻</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Marriage</FieldName>
      <FieldAlias>员工信息_婚姻</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.照片</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpPhoto</FieldName>
      <FieldAlias>员工信息_照片</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>IMAGE</FormatType>
        <FormatCode>OneInch</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工信息.籍贯</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>NativePlace</FieldName>
      <FieldAlias>员工信息_籍贯</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.民族</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Nation</FieldName>
      <FieldAlias>员工信息_民族</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.职务级别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>JobGrade</FieldName>
      <FieldAlias>员工信息_职务级别</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.身份证号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IDCard</FieldName>
      <FieldAlias>员工信息_身份证号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.政治面貌</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Political</FieldName>
      <FieldAlias>员工信息_政治面貌</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.学历</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Education</FieldName>
      <FieldAlias>员工信息_学历</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.聘任职称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmploymentTitle</FieldName>
      <FieldAlias>员工信息_聘任职称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人电话</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmergencyPhone</FieldName>
      <FieldAlias>员工信息_紧急联系人电话</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Emergency</FieldName>
      <FieldAlias>员工信息_紧急联系人</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人电话1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmergencyPhone</FieldName>
      <FieldAlias>员工信息_紧急联系人电话1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.邮编</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ZipCode</FieldName>
      <FieldAlias>员工信息_邮编</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.EmpCode = @员工编号</Condition>
      <ConditionParams />
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>EmpCode</DataSourceField>
            <DataSourceFieldAlias>员工编码</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@员工编号|REFERENCE|Employee|EmpCode|EmpName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','2b9911e68819cecb510e',0,NULL,NULL,'2016-06-06 11:50:29','9999-12-31 23:59:59',0,636008131137980525,NULL,NULL,'2016-06-06 11:50:29',NULL,NULL,'2016-06-06 12:31:53')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('2ba011e68819168f256d','199c11e6a1a644bef110','测试_员工卡','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>2ba011e68819168f256d</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.员工编码</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCode</FieldName>
      <FieldAlias>员工信息_员工编码</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.职位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpPosition</FieldName>
      <FieldAlias>员工信息_职位</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Repeat</ShowType>
      <TableType>MainTable</TableType>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','2ba011e68819168f256e',0,NULL,NULL,'2016-06-06 12:35:26','9999-12-31 23:59:59',0,636008138317650525,NULL,NULL,'2016-06-06 12:35:26',NULL,NULL,'2016-06-06 12:43:51')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('2cb011e6a2501bf6b694','199c11e6a1a644bef110','测试_员工履历表','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>2cb011e6a2501bf6b694</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.年龄</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Age</FieldName>
      <FieldAlias>员工信息_年龄</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.婚姻</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Marriage</FieldName>
      <FieldAlias>员工信息_婚姻</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.籍贯</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>NativePlace</FieldName>
      <FieldAlias>员工信息_籍贯</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.民族</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Nation</FieldName>
      <FieldAlias>员工信息_民族</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.政治面貌</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Political</FieldName>
      <FieldAlias>员工信息_政治面貌</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.学历</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Education</FieldName>
      <FieldAlias>员工信息_学历</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.聘任职称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmploymentTitle</FieldName>
      <FieldAlias>员工信息_聘任职称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.身份证号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IDCard</FieldName>
      <FieldAlias>员工信息_身份证号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.手机</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Mobile</FieldName>
      <FieldAlias>员工信息_手机</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.邮编</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ZipCode</FieldName>
      <FieldAlias>员工信息_邮编</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Emergency</FieldName>
      <FieldAlias>员工信息_紧急联系人</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.紧急联系人电话</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmergencyPhone</FieldName>
      <FieldAlias>员工信息_紧急联系人电话</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.职务</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpJob</FieldName>
      <FieldAlias>员工信息_职务</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.照片</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpPhoto</FieldName>
      <FieldAlias>员工信息_照片</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>IMAGE</FormatType>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.开始日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>StartDate</FieldName>
      <FieldAlias>员工学历子集_开始日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date1</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.结束日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EndDate</FieldName>
      <FieldAlias>员工学历子集_结束日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date1</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.专业</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Major</FieldName>
      <FieldAlias>员工学历子集_专业</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工学历子集.学历</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Education</FieldName>
      <FieldAlias>员工学历子集_学历</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.家庭住址2</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>HomeAddress</FieldName>
      <FieldAlias>员工信息_家庭住址2</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>学历学位.毕业院校</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>University</FieldName>
      <FieldAlias>学历学位_毕业院校</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.开始日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>StartDate</FieldName>
      <FieldAlias>工作经历_开始日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.结束日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EndDate</FieldName>
      <FieldAlias>工作经历_结束日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.工作单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkUnit</FieldName>
      <FieldAlias>工作经历_工作单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.成员姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>MemberName</FieldName>
      <FieldAlias>家庭成员_成员姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.与本人关系</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Relations</FieldName>
      <FieldAlias>家庭成员_与本人关系</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.出生日期</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DateOfBirth</FieldName>
      <FieldAlias>家庭成员_出生日期</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
      <CellFormat>
        <FormatType>DATETIME</FormatType>
        <FormatCode>Date3</FormatCode>
      </CellFormat>
    </RptCell>
    <RptCell>
      <Data>家庭成员.工作单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkUnit</FieldName>
      <FieldAlias>家庭成员_工作单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>家庭成员.工作职务</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkJob</FieldName>
      <FieldAlias>家庭成员_工作职务</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称系列</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>TitleSeries</FieldName>
      <FieldAlias>职称_职称系列</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称专业</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Professional</FieldName>
      <FieldAlias>职称_职称专业</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.职称等级1</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>GradeTitle</FieldName>
      <FieldAlias>职称_职称等级1</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.获取时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IssueDate</FieldName>
      <FieldAlias>职称_获取时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>职称.有效时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ValideDate</FieldName>
      <FieldAlias>职称_有效时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>工作经历.职务/职级</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>WorkJob</FieldName>
      <FieldAlias>工作经历_职务/职级</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.证书名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Name</FieldName>
      <FieldAlias>资格证书_证书名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.专业名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>ProfessionalName</FieldName>
      <FieldAlias>资格证书_专业名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.证书编号</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>CertificateNumber</FieldName>
      <FieldAlias>资格证书_证书编号</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.获取证书时间</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>IssueDate</FieldName>
      <FieldAlias>资格证书_获取证书时间</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>资格证书.注册单位</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>RegisterUnit</FieldName>
      <FieldAlias>资格证书_注册单位</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>MainTable</TableType>
      <Condition>Employee.Fid = @员工</Condition>
      <ConditionParams>
        <string>员工编码</string>
      </ConditionParams>
      <FilterCondition>
        <Condtions>
          <RptConditionItem>
            <DataSource>Employee</DataSource>
            <DataSourceAlias>员工信息</DataSourceAlias>
            <DataSourceField>Fid</DataSourceField>
            <DataSourceFieldAlias>唯一标识</DataSourceFieldAlias>
            <Operator>EQ</Operator>
            <ConditionValue>
              <ValueCtrlType>PARAM</ValueCtrlType>
              <Value>@员工|REFERENCE|Employee|Fid|EmpName</Value>
            </ConditionValue>
            <RelationOperator>AND</RelationOperator>
          </RptConditionItem>
        </Condtions>
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubEducation</DataSourceName>
      <DataSourceAlias>员工学历子集</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubEducation</SubTable>
        <SubTableAlias>员工学历子集</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubCareer</DataSourceName>
      <DataSourceAlias>工作经历</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubCareer</SubTable>
        <SubTableAlias>工作经历</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubFamily</DataSourceName>
      <DataSourceAlias>家庭成员</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubFamily</SubTable>
        <SubTableAlias>家庭成员</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubTitle</DataSourceName>
      <DataSourceAlias>职称</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>SubTable</TableType>
      <MainSubRelation>
        <MainTable>Employee</MainTable>
        <MainTableAlias>员工信息</MainTableAlias>
        <MainField>Fid</MainField>
        <SubTable>EmpSubTitle</SubTable>
        <SubTableAlias>职称</SubTableAlias>
        <SubField>EmpUid</SubField>
      </MainSubRelation>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>EmpSubCertificate</DataSourceName>
      <DataSourceAlias>资格证书</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>SubTable</TableType>
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','2cb011e6a2501bf6b695',0,NULL,NULL,'2016-06-24 10:52:38','9999-12-31 23:59:59',0,636023623594740905,NULL,NULL,'2016-06-07 21:02:38',NULL,NULL,'2016-06-24 10:52:39')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('393c11e6812e7c7872f0','199c11e6a1a644bef110','20160623','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TemplateFid>393c11e6812e7c7872f0</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.性别</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>Gender</FieldName>
      <FieldAlias>员工信息_性别</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>奖金总额.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>BonusTotal</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>奖金总额_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>MainTable</TableType>
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
    <RptDataSource>
      <DataSourceName>BonusTotal</DataSourceName>
      <DataSourceAlias>奖金总额</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>SubTable</TableType>
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
</RptTemplate>','393c11e6812e7c7872f1',0,NULL,NULL,'2016-06-26 16:06:10','9999-12-31 23:59:59',0,636025539718722202,NULL,NULL,'2016-06-23 20:17:44',NULL,NULL,'2016-06-26 16:06:11')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('4d6011e6989ebb1c647c','199c11e6a1a644bef110','统计用户个数','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>4d6011e6989ebb1c647c</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工信息.姓名</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpName</FieldName>
      <FieldAlias>员工信息_姓名</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.姓名拼音</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpPinYin</FieldName>
      <FieldAlias>员工信息_姓名拼音</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工信息.部门</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>DeptUid</FieldName>
      <FieldAlias>员工信息_部门</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>Employee</DataSourceName>
      <DataSourceAlias>员工信息</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>Table</ShowType>
      <TableType>MainTable</TableType>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
  <SqlCells>
    <RptSqlCell>
      <SQL>SELECT count(*) as content FROM FapUser </SQL>
      <Title>统计用户</Title>
    </RptSqlCell>
    <RptSqlCell>
      <SQL>select count(*)  as content from Employee
where DeptUid=#{人员所在部门|REFERENCE|OrgDept|Fid|DeptName}</SQL>
      <Title>统计人员</Title>
    </RptSqlCell>
    <RptSqlCell>
      <SQL>select count(*)  as content from OrgDept 
where Fid=#{部门|REFERENCE|OrgDept|Fid|DeptName}</SQL>
      <Title>统计部门</Title>
    </RptSqlCell>
    <RptSqlCell>
      <SQL>select top 1 ''{{year}}'' as content from FapUser</SQL>
      <Title>当前年份</Title>
    </RptSqlCell>
  </SqlCells>
</RptTemplate>','4d6011e6989ebb1c647d',0,NULL,NULL,'2016-07-20 11:19:33','9999-12-31 23:59:59',0,636046103745029190,NULL,NULL,'2016-07-19 11:27:36',NULL,NULL,'2016-07-20 11:19:34')
GO
INSERT INTO `RptSimpleTemplate`(`Fid`,`Pid`,`ReportName`,`DataXml`,`XlsFile`,`IsDir`,`OrgUid`,`GroupUid`,`EnableDate`,`DisableDate`,`Dr`,`Ts`,`CreateBy`,`CreateName`,`CreateDate`,`UpdateBy`,`UpdateName`,`UpdateDate`) VALUES('9fed11e682b9dda2e0fe','199c11e6a1a644bef110','009','<?xml version="1.0" encoding="utf-16"?>
<RptTemplate xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TemplateFid>9fed11e682b9dda2e0fe</TemplateFid>
  <SheetName>Sheet1</SheetName>
  <Cells>
    <RptCell>
      <Data>员工离职.员工</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpBizResign</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpUid</FieldName>
      <FieldAlias>员工离职_员工</FieldAlias>
      <WithMC>true</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工离职.员工编码</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpBizResign</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>EmpCode</FieldName>
      <FieldAlias>员工离职_员工编码</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
    <RptCell>
      <Data>员工离职.单据名称</Data>
      <IsFormula>false</IsFormula>
      <DataSourceName>EmpBizResign</DataSourceName>
      <DataSourceType>Table</DataSourceType>
      <FieldName>BillName</FieldName>
      <FieldAlias>员工离职_单据名称</FieldAlias>
      <WithMC>false</WithMC>
      <FillType>Full</FillType>
    </RptCell>
  </Cells>
  <DataSource>
    <RptDataSource>
      <DataSourceName>EmpBizResign</DataSourceName>
      <DataSourceAlias>员工离职</DataSourceAlias>
      <DataSourceType>Table</DataSourceType>
      <ShowType>None</ShowType>
      <TableType>MainTable</TableType>
      <DataSourceFieldOrder>
        <FieldName>EmpCode</FieldName>
        <FieldOrder>Asc</FieldOrder>
      </DataSourceFieldOrder>
      <Condition />
      <ConditionParams />
      <FilterCondition>
        <Condtions />
      </FilterCondition>
    </RptDataSource>
  </DataSource>
  <Condition>
    <Condtions />
  </Condition>
  <SqlCells />
</RptTemplate>','9fed11e682b9dda2e0ff',0,NULL,NULL,'2016-11-01 12:54:19','9999-12-31 23:59:59',0,636136016602303518,NULL,NULL,'2016-11-01 12:44:26',NULL,NULL,'2016-11-01 12:54:20')
GO


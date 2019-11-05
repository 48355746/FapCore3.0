using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.MetaData
{
    /// <summary>
    /// 字段元数据定义
    /// by sunchangtan
    /// </summary>
    [Serializable]
    [Table("FapColumn")]
    public class FapColumn : BaseModel
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 字段名 
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string ColComment { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string ColDefault { get; set; }
        /// <summary>
        /// 默认值生成类名， 但是ColDefault优先级高
        /// </summary>
        public string DefaultValueClass { get; set; }
        /// <summary>
        /// 字段类型， PK, UID, STRING,INT、DOUBLE、DATETIME、BLOB、CLOB、BOOL、LONG
        /// </summary>
        public string ColType { get; set; }
        /// <summary>
        /// 类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ColTypeMC { get; set; }
        /// <summary>
        /// 类型为数字的时候 控制最小值
        /// </summary>
        public int? MinValue { get; set; }
        /// <summary>
        /// 类型为数字的时候 控制最大值
        /// </summary>
        public int? MaxValue { get; set; }
        /// <summary>
        /// 字段属性（标识主键0，固定字段1，扩展字段2，特性字段3）
        /// </summary>
        public int ColProperty { get; set; }
        /// <summary>
        /// 属性 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ColPropertyMC { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int ColLength { get; set; }
        /// <summary>
        /// 显示宽度grid用
        /// </summary>
        public int DisplayWidth { get; set; }
        /// <summary>
        /// 字段排序
        /// </summary>
        public int ColOrder { get; set; }
        /// <summary>
        /// 字段精度，针对数值型，默认是0
        /// </summary>
        public int ColPrecision { get; set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public int NullAble { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int ShowAble { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public int EditAble { get; set; }
        /// <summary>
        /// 是否是默认字段
        /// </summary>
        public int IsDefaultCol { get; set; }
        /// <summary>
        /// 控件类型，TEXT、DOUBLE、MONEY、INT、DATETIME、DATE、COMBOBOX、RADIO、CHECKBOX、MEMO、FILE, REFERECE, IMAGE, CHECKBOXLIST
        /// </summary>
        public string CtrlType { get; set; }
        /// <summary>
        /// 控件类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string CtrlTypeMC { get; set; }
        /// <summary>
        /// 附件个数[1个或多个]
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// 附件扩展类型
        /// </summary>
        public string FileSuffix { get; set; }
        /// <summary>
        /// 附件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 显示格式
        /// </summary>
        public string DisplayFormat { get; set; }
        /// <summary>
        /// 参照对应的表名
        /// </summary>
        public string RefTable { get; set; }
        /// <summary>
        /// 参照对应的ID字段
        /// </summary>
        public string RefID { get; set; }
        /// <summary>
        /// 参照对应组件Fid，参照类型为 组件FapComponent的时候 取这个值
        /// </summary>
        public string RefCode { get; set; }
        /// <summary>
        /// 参照对应的显示字段名
        /// </summary>
        public string RefName { get; set; }
        /// <summary>
        /// 参照的条件
        /// </summary>
        public string RefCondition { get; set; }
        /// <summary>
        /// 参照的字段
        /// </summary>
        public string RefCols { get; set; }
        /// <summary>
        /// 参照返回值映射
        /// </summary>
        public string RefReturnMapping { get; set; }
        /// <summary>
        /// 是否多选（Combobox,Reference）
        /// </summary>
        public int MultiAble { get; set; }
        /// <summary>
        /// 树参照节点图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 参照类型：TREE树形参照，GRID表格参照，TREEGRID树表参照
        /// </summary>
        public string RefType { get; set; }
        /// <summary>
        /// 主表表名
        /// </summary>
        public string MainTable { get; set; }
        /// <summary>
        /// 主表字段
        /// </summary>
        public string MainTableCol { get; set; }
        /// <summary>
        /// 计算表达式
        /// </summary>
        public string CalculationExpr { get; set; }
        /// <summary>
        /// 列分组
        /// </summary>
        public string ColGroup { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string ComponentUid { get; set; }
        /// <summary>
        /// 组件 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ComponentUidMC { get; set; }
        /// <summary>
        /// 作废：1表示作废，0表示非作废
        /// </summary>
        public int IsObsolete { get; set; }
        /// <summary>
        /// 多语字段标识
        /// </summary>
        public int IsMultiLang { get; set; }
        /// <summary>
        /// 远程校验URL
        /// </summary>
        public string RemoteChkURL { get; set; }
        /// <summary>
        /// 远程校验提醒信息
        /// </summary>
        public string RemoteChkMsg { get; set; }
        /// <summary>
        /// 多语-中文繁体
        /// </summary>
        public string LangZhTW { get; set; }
        /// <summary>
        /// 多语-英文
        /// </summary>
        public string LangEn { get; set; }
        /// <summary>
        /// 多语-日文
        /// </summary>
        public string LangJa { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }

        #region 计算属性
        /// <summary>
        /// 用于生成jqgrid列的格式化
        /// </summary>
        [Computed]
        public string CustomFormat { get; set; }

        /// <summary>
        /// 是否是自定义的列
        /// </summary>
        [Computed]
        public int IsCustomColumn { get; set; }

        #endregion

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 是否数值类型
        /// </summary>
        /// <returns></returns>
        public bool IsNumericType()
        {
            return this.ColType == FapColumn.COL_TYPE_INT || this.ColType == FapColumn.COL_TYPE_LONG || this.ColType == FapColumn.COL_TYPE_DOUBLE || this.ColType == FapColumn.COL_TYPE_BLOB;
        }
        /// <summary>
        /// 是否整型
        /// </summary>
        /// <returns></returns>
        public bool IsIntType()
        {
            return this.ColType == FapColumn.COL_TYPE_INT || this.ColType == FapColumn.COL_TYPE_BLOB;
        }
        /// <summary>
        /// 是否浮点型
        /// </summary>
        /// <returns></returns>
        public bool IsDoubleType()
        {
            return this.ColType == FapColumn.COL_TYPE_DOUBLE;
        }
        /// <summary>
        /// 是否长整型
        /// </summary>
        /// <returns></returns>
        public bool IsLongType()
        {
            return this.ColType == FapColumn.COL_TYPE_LONG;
        }
        public override bool Equals(object obj)
        {
            FapColumn fapColumn = obj as FapColumn;
            return fapColumn.TableName == this.TableName && fapColumn.Id == this.Id
                && fapColumn.Fid == this.Fid && fapColumn.ColName == this.ColName;
        }
        public override int GetHashCode()
        {
            int hashCode = 17; // we *know* we are using this in a dictionary, so pre-compute this
            hashCode = hashCode * 23 + Id.GetHashCode();
            hashCode = hashCode * 23 + (Fid == null ? 0 : Fid.GetHashCode());
            hashCode = hashCode * 23 + (EnableDate == null ? 0 : EnableDate.GetHashCode());
            hashCode = hashCode * 23 + (DisableDate == null ? 0 : DisableDate.GetHashCode());
            hashCode = hashCode * 23 + Dr.GetHashCode();
            hashCode = hashCode * 23 + Ts.GetHashCode();
            hashCode = hashCode * 23 + (CreateBy == null ? 0 : CreateBy.GetHashCode());
            hashCode = hashCode * 23 + (CreateDate == null ? 0 : CreateDate.GetHashCode());
            hashCode = hashCode * 23 + (CreateName == null ? 0 : CreateName.GetHashCode());
            hashCode = hashCode * 23 + (UpdateBy == null ? 0 : UpdateBy.GetHashCode());
            hashCode = hashCode * 23 + (UpdateDate == null ? 0 : UpdateDate.GetHashCode());
            hashCode = hashCode * 23 + (UpdateName == null ? 0 : UpdateName.GetHashCode());
            hashCode = hashCode * 23 + (TableName == null ? 0 : TableName.GetHashCode());
            hashCode = hashCode * 23 + (ColName == null ? 0 : ColName.GetHashCode());
            hashCode = hashCode * 23 + (ColComment == null ? 0 : ColComment.GetHashCode());
            hashCode = hashCode * 23 + (ColDefault == null ? 0 : ColDefault.GetHashCode());
            hashCode = hashCode * 23 + (ColType == null ? 0 : ColType.GetHashCode());
            hashCode = hashCode * 23 + ColProperty.GetHashCode();
            hashCode = hashCode * 23 + ColLength.GetHashCode();
            hashCode = hashCode * 23 + DisplayWidth.GetHashCode();
            hashCode = hashCode * 23 + ColOrder.GetHashCode();
            hashCode = hashCode * 23 + ColPrecision.GetHashCode();
            hashCode = hashCode * 23 + NullAble.GetHashCode();
            hashCode = hashCode * 23 + ShowAble.GetHashCode();
            hashCode = hashCode * 23 + IsDefaultCol.GetHashCode();
            hashCode = hashCode * 23 + (CtrlType == null ? 0 : CtrlType.GetHashCode());
            hashCode = hashCode * 23 + (RefTable == null ? 0 : RefTable.GetHashCode());
            hashCode = hashCode * 23 + (RefID == null ? 0 : RefID.GetHashCode());
            hashCode = hashCode * 23 + (RefCode == null ? 0 : RefCode.GetHashCode());
            hashCode = hashCode * 23 + (RefName == null ? 0 : RefName.GetHashCode());
            hashCode = hashCode * 23 + (RefCondition == null ? 0 : RefCondition.GetHashCode());
            hashCode = hashCode * 23 + (RefCols == null ? 0 : RefCols.GetHashCode());
            hashCode = hashCode * 23 + (RefType == null ? 0 : RefType.GetHashCode());
            hashCode = hashCode * 23 + (MainTable == null ? 0 : RefType.GetHashCode());
            hashCode = hashCode * 23 + (MainTableCol == null ? 0 : MainTableCol.GetHashCode());
            hashCode = hashCode * 23 + (CalculationExpr == null ? 0 : CalculationExpr.GetHashCode());
            return hashCode;
        }
        /// <summary>
        /// 主键 
        /// </summary>
        public const string COL_TYPE_PK = "PK";
        /// <summary>
        /// Fid
        /// </summary>
        public const string COL_TYPE_UID = "UID";
        /// <summary>
        /// 字符串
        /// </summary>
        public const string COL_TYPE_STRING = "STRING";
        /// <summary>
        /// INT
        /// </summary>
        public const string COL_TYPE_INT = "INT";
        /// <summary>
        /// long
        /// </summary>
        public const string COL_TYPE_LONG = "LONG";
        /// <summary>
        /// double
        /// </summary>
        public const string COL_TYPE_DOUBLE = "DOUBLE";
        /// <summary>
        /// 日期
        /// </summary>
        public const string COL_TYPE_DATETIME = "DATETIME";
        /// <summary>
        /// 布尔
        /// </summary>
        public const string COL_TYPE_BOOL = "BOOL";
        /// <summary>
        /// 长文本
        /// </summary>
        public const string COL_TYPE_CLOB = "CLOB";
        /// <summary>
        /// blob
        /// </summary>
        public const string COL_TYPE_BLOB = "BLOB";

        /// <summary>
        /// 文本框
        /// </summary>
        public const string CTRL_TYPE_TEXT = "TEXT";
        /// <summary>
        /// 密码
        /// </summary>
        public const string CTRL_TYPE_PASSWORD = "PASSWORD";
        /// <summary>
        /// 小数
        /// </summary>
        public const string CTRL_TYPE_DOUBLE = "DOUBLE";
        /// <summary>
        /// 金额
        /// </summary>
        public const string CTRL_TYPE_MONEY = "MONEY";
        /// <summary>
        /// 整数
        /// </summary>
        public const string CTRL_TYPE_INT = "INT";
        /// <summary>
        /// 日期时间
        /// </summary>
        public const string CTRL_TYPE_DATETIME = "DATETIME";
        /// <summary>
        /// 日期
        /// </summary>
        public const string CTRL_TYPE_DATE = "DATE";
        /// <summary>
        /// 复选框
        /// </summary>
        public const string CTRL_TYPE_CHECKBOX = "CHECKBOX";
        /// <summary>
        /// 单选框
        /// </summary>
        public const string CTRL_TYPE_RADIO = "RADIO";
        /// <summary>
        /// 下拉框
        /// </summary>
        public const string CTRL_TYPE_COMBOBOX = "COMBOBOX";
        /// <summary>
        /// 多行文本
        /// </summary>
        public const string CTRL_TYPE_MEMO = "MEMO";
        /// <summary>
        /// 附件
        /// </summary>
        public const string CTRL_TYPE_FILE = "FILE";
        /// <summary>
        /// 参照
        /// </summary>
        public const string CTRL_TYPE_REFERENCE = "REFERENCE";
        /// <summary>
        /// 图片同附件
        /// </summary>
        public const string CTRL_TYPE_IMAGE = "IMAGE";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string CTRL_TYPE_EMAIL = "EMAIL";
        /// <summary>
        /// 电话
        /// </summary>
        public const string CTRL_TYPE_PHONE = "PHONE";
        /// <summary>
        /// 多选列表
        /// </summary>
        public const string CTRL_TYPE_CHECKBOXLIST = "CHECKBOXLIST";
        /// <summary>
        /// 富文本
        /// </summary>
        public const string CTRL_TYPE_RICHTEXTBOX = "RICHTEXTBOX";
        /// <summary>
        /// 城市
        /// </summary>
        public const string CTRL_TYPE_CITY = "CITY";
        /// <summary>
        /// 籍贯
        /// </summary>
        public const string CTRL_TYPE_NATIVE = "NATIVE";
        /// <summary>
        /// 评星级
        /// </summary>
        public const string CTRL_TYPE_STAR = "STAR";


    }
}

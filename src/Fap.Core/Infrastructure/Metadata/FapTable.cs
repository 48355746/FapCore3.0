using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 表的元数据定义
    /// by sunchangtan
    /// </summary>
    [Serializable]
    [Table("FapTable")]
    public class FapTable : BaseModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableComment { get; set; }
        /// <summary>
        /// 描述用途
        /// </summary>
        public string TableDesc { get; set; }
        /// <summary>
        /// 表类型
        /// </summary>
        public string TableType { get; set; }
        /// <summary>
        /// 类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableTypeMC { get; set; }
        /// <summary>
        /// 表分类
        /// </summary>
        public string TableCategory { get; set; }
        /// <summary>
        /// 分类 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableCategoryMC { get; set; }
        /// <summary>
        /// 表模式，single单表、 primary主表,child 子表, extension 扩展表
        /// </summary>
        public string TableMode { get; set; }
        /// <summary>
        /// 表模式 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableModeMC { get; set; }
        /// <summary>
        /// 子表， 以分号为分隔符
        /// </summary>
        public string SubTable { get; set; }
        /// <summary>
        /// 扩展表
        /// </summary>
        public string ExtTable { get; set; }
        /// <summary>
        /// 是否是树
        /// </summary>
        public int IsTree { get; set; }
        /// <summary>
        /// 是否分页
        /// </summary>
        public int IsPagination { get; set; }
        /// <summary>
        /// 是否同步
        /// </summary>
        public int IsSync { get; set; }
        /// <summary>
        /// 表特性
        /// </summary>
        public string TableFeature { get; set; }
        /// <summary>
        /// 数据拦截器类
        /// </summary>
        public string DataInterceptor { get; set; }
        /// <summary>
        /// 脚本注入器
        /// </summary>
        public string ScriptInjection { get; set; }
        /// <summary>
        /// sql注入器
        /// </summary>
        public string SqlInjection { get; set; }
        /// <summary>
        /// 历史追溯： 1表示历史可追溯， 0表示不可追溯。 默认为0；
        /// </summary>
        public int TraceAble { get; set; }
        /// <summary>
        /// 所属产品
        /// </summary>
        public string ProductUid { get; set; }
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

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
        public override bool Equals(object obj)
        {
            FapTable fapTable = obj as FapTable;
            return fapTable.Id == this.Id && fapTable.Fid == this.Fid
                &&fapTable.TableName == this.TableName;
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
            hashCode = hashCode * 23 + (TableComment == null ? 0 : TableComment.GetHashCode());
            hashCode = hashCode * 23 + (TableMode == null ? 0 : TableMode.GetHashCode());
            hashCode = hashCode * 23 + (SubTable == null ? 0 : SubTable.GetHashCode());
            hashCode = hashCode * 23 + IsTree.GetHashCode();
            hashCode = hashCode * 23 + IsPagination.GetHashCode();
            hashCode = hashCode * 23 + IsSync.GetHashCode();

            return hashCode;
        }

        public const string TABLE_MODE_SINGLE = "SINGLE";
        public const string TABLE_MODE_PRIMARY = "PRIMARY";
        public const string TABLE_MODE_CHILD = "CHILD";
        public const string TABLE_MODE_EXTENSION = "EXTENSION";
    }
}

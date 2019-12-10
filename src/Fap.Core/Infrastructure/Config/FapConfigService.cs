using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 业务配置
    /// 单据编码，单据回写
    /// </summary>
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class FapConfigService : IFapConfigService
    {
        private  object obj = new object();
        private readonly ILogger<FapConfigService> _logger;
        private IFapPlatformDomain _appDomain;
        private IDbContext _dbContext;
        public FapConfigService(IDbContext accessor, ILogger<FapConfigService> logger, IFapPlatformDomain appDomain)
        {
            _dbContext = accessor;
            _logger = logger;
            _appDomain = appDomain;
        }
        [Transactional]
        public string ConfigGroupOperation(string operation, string id, string parent, string text)
        {
            string result = "0";
            if (operation ==TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.Count("FapConfig", "ConfigGroup=@ConfigGroup", new DynamicParameters(new { ConfigGroup = id }));
                if (c == 0)
                {
                    result = "" + _dbContext.Execute("delete from FapConfigGroup where Fid=@Id", new DynamicParameters(new { Id = id }));
                }
            }
            else if (operation ==TreeNodeOper.CREATE_NODE)
            {
                dynamic fdo = new FapDynamicObject("FapConfigGroup");
                fdo.Pid = id;
                fdo.CfName = text;
                long rv = _dbContext.InsertDynamicData(fdo);
                result = fdo.Fid;
            }
            else if (operation ==TreeNodeOper.RENAME_NODE)
            {
                result = "" + _dbContext.Execute("update FapConfigGroup set CfName=@CfName where Fid=@Id", new DynamicParameters(new { CfName = text, Id = id }));
            }
            else if (operation ==TreeNodeOper.MOVE_NODE)
            {
                result = "" + _dbContext.Execute("update FapConfigGroup set Pid=@Pid where Fid=@Id", new DynamicParameters(new { Pid = parent, Id = id }));
            }
            else if (operation ==TreeNodeOper.COPY_NODE)
            {

            }

            return result;
        }

        /// <summary>
        /// 获取系统配置参数值
        /// </summary>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        public string GetSysParamValue(string paramKey)
        {
            FapConfig config;
            if (_appDomain.SysParamSet.TryGetValueByKey(paramKey, out config))
            {
                return config.ParamValue;
            }
            return "";
        }
        /// <summary>
        /// 获取自增序号
        /// </summary>
        /// <param name="seqName">唯一名称</param>
        /// <param name="stepBy">步长</param>
        /// <returns></returns>
        public int GetSequence(string seqName, int stepBy = 1)
        {
            lock (obj)
            {
                //DynamicParameters param = new DynamicParameters();
                //param.Add("SeqName", seqName);
                CfgSequenceRule sr = _dbContext.QueryFirstOrDefault<CfgSequenceRule>("select * from CfgSequenceRule where SeqName=@SeqName", new DynamicParameters(new { SeqName = seqName }));
                if (sr != null)
                {
                    int currValue = sr.CurrValue + sr.StepBy;
                    string sql = $"update CfgSequenceRule set CurrValue={currValue} where id={sr.Id}";
                    _dbContext.Execute(sql);
                    return sr.CurrValue;
                }
                else
                {
                    CfgSequenceRule cs = new CfgSequenceRule { SeqName = seqName, MinValue = 0, StepBy = stepBy, CurrValue = 1 };
                    _dbContext.Insert<CfgSequenceRule>(sr);
                    return 1;
                }
            }
        }
        /// <summary>
        /// 获取设置的编码
        /// </summary>
        /// <param name="tableName">实体名称</param>
        /// <returns></returns>

        public Dictionary<string, string> GetBillCode(string tableName)
        {
            Dictionary<string, string> dictCodes = new Dictionary<string, string>();

            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", tableName);
            IEnumerable<CfgBillCodeRule> bcs = _dbContext.QueryWhere<CfgBillCodeRule>("BillEntity=@TableName", param);
            if (bcs == null)
            {
                bcs = new List<CfgBillCodeRule>();
            }
            if (bcs.Any())
            {
                foreach (var bc in bcs)
                {
                    string prefix = bc.Prefix;
                    string date = string.Empty;
                    string dateformat = bc.DateFormat;
                    if (dateformat.IsPresent())
                    {
                        date =DateTime.Now.ToString(dateformat);
                    }
                    string seqName = bc.BillEntity + "_" + bc.FieldName;
                    if (bc.ReCountContidion.IsPresent())
                    {
                        if (bc.ReCountContidion.EqualsWithIgnoreCase("year"))
                        {
                            seqName += DateTime.Now.ToString("yyyy");
                        }
                        else if (bc.ReCountContidion.EqualsWithIgnoreCase("month"))
                        {
                            seqName += DateTime.Now.ToString("yyyyMM");
                        }
                        else
                        {
                            seqName += DateTime.Now.ToString("yyyyMMdd");
                        }
                    }
                    int seq = GetSequence(seqName);
                    int totalWidth = 0;
                    if (bc.SequenceLen > 0)
                    {
                        totalWidth = bc.SequenceLen;
                    }
                    string symbol = bc.Symbol;
                    if (symbol.IsMissing())
                    {
                        symbol = "0";
                    }
                    bc.BillCode = prefix + date + seq.ToString().PadLeft(totalWidth, Convert.ToChar(symbol));
                    dictCodes.Add(bc.FieldName, bc.BillCode);
                }
            }
            else
            {
                //单据没有配置的时候 返回默认的值
                if (_appDomain.TableSet.TryGetValueByName(tableName,out FapTable table))
                {
                    if (table.TableFeature.Contains("BillFeature"))
                    {
                        //CfgBillCodeRule bc = new CfgBillCodeRule();
                        //bc.FieldName = "BillCode";
                        int seq = GetSequence(tableName);
                        //bc.BillCode = seq.ToString().PadLeft(7, '0');
                        string billcode = seq.ToString().PadLeft(7, '0');
                        dictCodes.Add("BillCode", billcode);
                    }
                }

            }
            return dictCodes;
        }
    }
}

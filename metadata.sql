--元数据多语言初始化
delete from FapMultiLanguage where Qualifier='FapColumn'

insert into FapMultiLanguage(Fid,Qualifier,LangKey,LangValue,LangValueZhCn,EnableDate,DisableDate,Dr,Ts) select Fid,'FapColumn',TableName+'_'+ColName,ColComment,ColComment,EnableDate,DisableDate,Dr,Ts  from FapColumn where IsDefaultCol=0 and Dr=0

delete from FapMultiLanguage where Qualifier='FapTable'

insert into FapMultiLanguage(Fid,Qualifier,LangKey,LangValue,LangValueZhCn,EnableDate,DisableDate,Dr,Ts) select Fid,'FapTable',TableName,TableComment,TableComment,EnableDate,DisableDate,Dr,Ts  from FapTable where Dr=0

delete from FapMultiLanguage where Qualifier='Menu'

insert into FapMultiLanguage(Fid,Qualifier,LangKey,LangValue,LangValueZhCn,EnableDate,DisableDate,Dr,Ts) select Fid,'Menu',Fid,MenuName,MenuName,EnableDate,DisableDate,Dr,Ts  from FapMenu where   Dr=0

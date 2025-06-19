/* forward select query*/
create function re_forward(@amount bigint, @source int, @ddate Date, @time varchar(6))
returns @finall_res table (
		vv varchar(10), trnD Date, trnT varchar(6), amnt bigint, sour int, dest int, branch int, descr varchar(50), depth int, omgh int)
begin
	declare @deepth int = 0;
	set @deepth = @deepth + 1;
	declare @am bigint = 0;
	declare @sr int = 0;
	declare @dt Date;
	declare @tm varchar(6);
	if exists (select * from dbo.front_select(@amount, @source, @ddate, @time))
		insert into @finall_res select *, @deepth, @deepth from dbo.front_select(@amount, @source, @ddate, @time)
	declare @cond varchar(10);
	set @cond = isnull((select top 1 temp.vv from @finall_res as temp where temp.depth <> 0 and isnull(temp.dest, 0) <> 0 order by temp.trnD, temp.trnT), '');
	update @finall_res set depth = 0 where @cond = vv
	while @cond <> ''
	begin
		set @am = (select temp.amnt from @finall_res as temp where temp.vv = @cond);
		set @sr = (select temp.dest from @finall_res as temp where temp.vv = @cond);
		set @dt = (select temp.trnD from @finall_res as temp where temp.vv = @cond);
		set @tm = (select temp.trnT from @finall_res as temp where temp.vv = @cond);
		set @deepth = @deepth + 1;
		if exists (select * from dbo.front_select(@am, @sr, @dt, @tm))
			insert into @finall_res select *, @deepth, @deepth from dbo.front_select(@am, @sr, @dt, @tm);
		set @cond = isnull((select top 1 temp.vv from @finall_res as temp where temp.depth <> 0 and isnull(temp.dest, 0) <> 0 order by temp.trnD, temp.trnT), '');
		if @cond <> ''
		begin
			update @finall_res set depth = 0 where @cond = vv
		end
	end
	return
end
/* backward select query*/
create function re_backward(@amount bigint, @source int, @ddate Date, @time varchar(6))
returns @finall_res table (
		vv varchar(10), trnD Date, trnT varchar(6), amnt bigint, sour int, dest int, branch int, descr varchar(50), depth int, emgh int)
begin
	declare @deepth int = 0;
	set @deepth = @deepth + 1;
	declare @am bigint = 0;
	declare @sr int = 0;
	declare @dt Date;
	declare @tm varchar(6);
	if exists (select * from dbo.back_select(@amount, @source, @ddate, @time))
		insert into @finall_res select *, @deepth, @deepth from dbo.back_select(@amount, @source, @ddate, @time)
	declare @cond varchar(10);
	set @cond = isnull((select top 1 temp.vv from @finall_res as temp where temp.depth <> 0 and isnull(temp.sour, 0) <> 0 order by temp.trnD, temp.trnT), '');
	update @finall_res set depth = 0 where @cond = vv
	while @cond <> ''
	begin
		set @am = (select temp.amnt from @finall_res as temp where temp.vv = @cond);
		set @sr = (select temp.sour from @finall_res as temp where temp.vv = @cond);
		set @dt = (select temp.trnD from @finall_res as temp where temp.vv = @cond);
		set @tm = (select temp.trnT from @finall_res as temp where temp.vv = @cond);
		set @deepth = @deepth + 1;
		if exists (select * from dbo.back_select(@am, @sr, @dt, @tm))
			insert into @finall_res select *, @deepth, @deepth from dbo.back_select(@am, @sr, @dt, @tm);
		set @cond = isnull((select top 1 temp.vv from @finall_res as temp where temp.depth <> 0 and isnull(temp.sour, 0) <> 0 order by temp.trnD, temp.trnT), '');
		if @cond <> ''
		begin
			update @finall_res set depth = 0 where @cond = vv
		end
	end
	return
end

/*back select and front select*/

create function front_select(@amount bigint, @source int, @ddate Date, @time varchar(6))
returns table
as
return
	select *  from (select * from [Trn_Src_Des] cc where cc.SourceDep = @source and (cc.TrnDate > @ddate or(cc.TrnDate = @ddate and cast(cc.TrnTime as int) >= cast(@time as int)))) as pp where pp.SourceDep = @source and pp.TrnDate = (select top 1 gg.TrnDate from [Trn_Src_Des] gg where gg.SourceDep = @source and (gg.TrnDate > @ddate or ( gg.TrnDate = @ddate and cast(gg.TrnTime as int) >= cast(@time as int))) order by gg.TrnDate, gg.TrnTime)
	union select dd.VoucherId, dd.TrnDate, dd.TrnTime, dd.Amount, dd.SourceDep, dd.DesDep, dd.Branch_ID, dd.Trn_Desc from (select * , sum(ff.Amount) over (order by ff.TrnDate, ff.TrnTime) as amount_cum from Trn_Src_Des as ff where ff.SourceDep = @source and (ff.TrnDate > @ddate or(ff.TrnDate = @ddate and cast(ff.TrnTime as int) >= cast(@time as int))) and (ff.TrnDate <> (select top 1 ee.TrnDate from [Trn_Src_Des] ee where ee.SourceDep = @source and (ee.TrnDate > @ddate or (ee.TrnDate = @ddate and cast(ee.TrnTime as int) >= cast(@time as int))) order by ee.TrnDate, ee.TrnTime) or ff.Amount <> @amount)) as dd where dd.amount_cum <= @amount + (@amount * 0.1)
	

create function back_select(@amount bigint, @source int, @ddate Date, @time varchar(6))
returns table
as
return
	select *  from (select * from [Trn_Src_Des] cc where cc.DesDep = @source and (cc.TrnDate < @ddate or(cc.TrnDate = @ddate and cast(cc.TrnTime as int) <= cast(@time as int)))) as pp where pp.DesDep = @source and pp.TrnDate = (select top 1 gg.TrnDate from [Trn_Src_Des] gg where gg.DesDep = @source and (gg.TrnDate < @ddate or ( gg.TrnDate = @ddate and cast(gg.TrnTime as int) <= cast(@time as int))) order by gg.TrnDate, gg.TrnTime)
	union select dd.VoucherId, dd.TrnDate, dd.TrnTime, dd.Amount, dd.SourceDep, dd.DesDep, dd.Branch_ID, dd.Trn_Desc from (select * , sum(ff.Amount) over (order by ff.TrnDate, ff.TrnTime) as amount_cum from Trn_Src_Des as ff where ff.DesDep = @source and (ff.TrnDate < @ddate or(ff.TrnDate = @ddate and cast(ff.TrnTime as int) <= cast(@time as int))) and (ff.TrnDate <> (select top 1 ee.TrnDate from [Trn_Src_Des] ee where ee.DesDep = @source and (ee.TrnDate < @ddate or (ee.TrnDate = @ddate and cast(ee.TrnTime as int) <= cast(@time as int))) order by ee.TrnDate, ee.TrnTime) or ff.Amount <> @amount)) as dd where dd.amount_cum <= @amount + (@amount * 0.1)


with temps as(select (cast(substring(NatCod,1,1) as int) * 10 +cast(substring(NatCod,2,1) as int) * 9 +cast(substring(NatCod,3,1) as int) * 8 +cast(substring(NatCod,4,1) as int) * 7 +cast(substring(NatCod,5,1) as int) * 6 +cast(substring(NatCod,6,1) as int) * 5 +cast(substring(NatCod,7,1) as int) * 4 +cast(substring(NatCod,8,1) as int) * 3 +cast(substring(NatCod,9,1) as int) * 2) % 11 as remind, NatCod, CID from dbo.Customer)
select Customer.CID, Customer.Name, Customer.NatCod, Customer.Birthdate, Customer.Address, Customer.Tel, case when currects.CID = Customer.CID then 1 else 0 end as NatCodeStatus from (select * from temps where temps.remind <=2  and cast(substring(temps.NatCod, 10, 1) as int) = temps.remind or temps.remind > 2 and cast(substring(temps.NatCod, 10, 1) as int) = 11 - temps.remind) as currects right join Customer on currects.CID = Customer.CID;


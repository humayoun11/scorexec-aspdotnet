Alter PROCEDURE [usp_GetSinglePlayerStatistics]
@paramPlayerId AS INT,
@paramSeason AS INT,
@paramMatchTypeId AS INT,
@paramTeamId as Int
AS
BEGIN
		SELECT  * 
			from(
				select
				COALESCE( PlayerPastRecords.TotalMatch,0) + COALESCE(count (PlayerScores.MatchId),0) as 'TotalMatch',
				COALESCE(PlayerPastRecords.TotalInnings,0) + COALESCE(count (CASE WHEN PlayerScores.IsPlayedInning = 1 THEN 1 ELSE NULL END),0) as 'TotalInnings',
				(COALESCE(PlayerPastRecords.TotalBatRuns,0) + COALESCE(sum (PlayerScores.Bat_Runs),0)) as 'TotalBatRuns',
				(COALESCE(PlayerPastRecords.TotalBatBalls,0) + COALESCE(sum (PlayerScores.Bat_Balls),0)) as 'TotalBatBalls',
				(COALESCE(PlayerPastRecords.TotalFours,0) + COALESCE(sum (PlayerScores.Four),0)) as 'TotalFours',
				(COALESCE(PlayerPastRecords.TotalSixes,0) + COALESCE(sum(PlayerScores.Six),0)) as 'TotalSixes',
				(COALESCE(PlayerPastRecords.TotalNotOut,0) + COALESCE(count(case when HowOutId = 7 or howOutId = 8 then 1 else null end),0)) as 'TotalNotOut',
				(COALESCE(PlayerPastRecords.GetBowled,0) + COALESCE(count(case when HowOutId = 2 then 1 else null end),0)) as 'GetBowled',
				(COALESCE(PlayerPastRecords.GetCatch,0) + COALESCE(count(case when HowOutId = 1 then 1 else null end),0)) as 'GetCatch',
				(COALESCE(PlayerPastRecords.GetStump,0) + COALESCE(count(case when HowOutId = 3 then 1 else null end),0)) as 'GetStump',
				(COALESCE(PlayerPastRecords.GetRunOut,0) + COALESCE(count(case when HowOutId = 4 then 1 else null end),0)) as 'GetRunOut',
				(COALESCE(PlayerPastRecords.GetHitWicket,0)+ COALESCE(count(case when HowOutId = 6 then 1 else null end),0)) as 'GetHitWicket',
				(COALESCE(PlayerPastRecords.GetLBW,0) + COALESCE(count(case when HowOutId = 5 then 1 else null end),0)) as 'GetLBW',
				(
				select top 1 (Ball_Runs) FROM PlayerScores where Wickets = (select max(wickets) from playerScores where playerId = @paramPlayerId)
				) 
				as 'BestBowlingFigureRuns',
				
				
				case when (COALESCE(PlayerPastRecords.BestScore,0) >= COALESCE(max (Bat_Runs),0))
				then COALESCE(PlayerPastRecords.BestScore,0)
				else COALESCE(max (Bat_Runs),0)
				End as 'HightScore',
				COALESCE(max (Wickets),0) as 'MostWickets',		
				(COALESCE(PlayerPastRecords.NumberOf50s,0) + COALESCE(COUNT(CASE WHEN Bat_Runs >= 50 THEN 1 ELSE NULL END),0)) AS 'NumberOf50s',
				(COALESCE(PlayerPastRecords.NumberOf100s,0) + COALESCE(COUNT(CASE WHEN Bat_Runs >= 100 THEN 1 ELSE NULL END),0)) AS 'NumberOf100s',
				Case When (COALESCE(PlayerPastRecords.TotalBatBalls,0) + COALESCE(sum(Bat_Balls),0)) is null  OR (COALESCE(PlayerPastRecords.TotalBatBalls,0) + COALESCE(sum(Bat_Balls),0)) = 0 
					THEN null
				    ELSE CAST(
								cast((COALESCE(PlayerPastRecords.TotalBatRuns,0) + COALESCE(sum (Bat_Runs),0)) as float) * 100 / 
								CAST((COALESCE(PlayerPastRecords.TotalBatBalls,0) + COALESCE(sum(Bat_Balls),0)) as float) AS numeric(36,2))
				END As 'StrikeRate',


				CASE WHEN (CAST((COALESCE(PlayerPastRecords.TotalInnings,0) + COALESCE(cOUNT(Case When IsPlayedInning ='1' Then 1 else null end),0)) as float)) - 
						  (Cast((COALESCE(PlayerPastRecords.TotalNotOut,0) + COALESCE(Count(case when HowOutId = 7 or howOutId = 8 then 1 else null end),0)) as float)) = 0
					THEN null
				    ELSE CAST(
								(Cast((COALESCE(PlayerPastRecords.TotalBatRuns,0) + COALESCE(sum (Bat_Runs),0)) as float)) / 
								((cast((COALESCE(PlayerPastRecords.TotalInnings,0) + COALESCE(COUNT(Case When IsPlayedInning = 1 Then 1 else null end),0))as float)) - 
								(cast((COALESCE(PlayerPastRecords.TotalNotOut,0) + COALESCE(COUNT (case when HowOutId = 7 or howOutId= 8 then 1 else null end),0))as float)))
							   AS numeric(36,2))
				END As 'BattingAverage',
				(cast(round(COALESCE(PlayerPastRecords.TotalOvers,0) + COALESCE(sum (Overs),0),1) as numeric(36,1))) as 'TotalOvers',
				(COALESCE(PlayerPastRecords.TotalBallRuns,0) + COALESCE(sum (Ball_Runs),0)) as 'TotalBallRuns',
				(COALESCE(PlayerPastRecords.TotalWickets,0) + COALESCE(sum (Wickets),0)) as 'TotalWickets',
				(COALESCE(PlayerPastRecords.TotalMaidens,0) + COALESCE(sum (Maiden),0)) as 'TotalMaidens',
				(COALESCE(PlayerPastRecords.DoBowled,0)) as 'DoBowled',
				(COALESCE(PlayerPastRecords.DoCatch,0)) as 'DoCatch',
				(COALESCE(PlayerPastRecords.DoHitWicket,0)) as 'DoHitWicket',
				(COALESCE(PlayerPastRecords.DoLBW,0)) as 'DoLBW',
				(COALESCE(PlayerPastRecords.DoStump,0)) as 'DoStump',
				CASE WHEN (COALESCE(PlayerPastRecords.TotalWickets,0) + COALESCE(sum(Wickets),0)) is null OR (COALESCE(PlayerPastRecords.TotalWickets,0) + COALESCE(sum(Wickets),0)) = 0 
					THEN null
					ELSE CAST((CAST((COALESCE(PlayerPastRecords.TotalBallRuns,0) + COALESCE(SUM(Ball_Runs),0))as float) / CAST((COALESCE(PlayerPastRecords.TotalWickets,0) + COALESCE(sum(Wickets),0)) as float)) AS numeric(36,2))
					END As 'BowlingAvg',

				--CASE WHEN (COALESCE(PlayerPastRecord.TotalOvers,0) + COALESCE(sum(Overs),0)) IS NULL OR (COALESCE(PlayerPastRecord.TotalOvers,0) + COALESCE(sum(Overs),0)) = 0 
				--	THEN null
				--ELSE CAST((CAST((COALESCE(PlayerPastRecord.TotalBallRuns,0) + COALESCE(SUM(Ball_Runs),0))as float) / CAST((COALESCE(PlayerPastRecord.TotalOvers,0) + COALESCE(sum(Overs),0)) as float)) AS numeric(36,2))
				--	END As 'Economy',

					CASE WHEN (COALESCE(PlayerPastRecords.TotalOvers,0) + COALESCE(sum(Overs),0)) IS NULL OR (COALESCE(PlayerPastRecords.TotalOvers,0) + COALESCE(sum(Overs),0)) = 0 
					THEN null
				ELSE 
						cast(((COALESCE(PlayerPastRecords.TotalBallRuns,0) + COALESCE(SUM(Ball_Runs),0)) / (floor(COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0)) * 6 + cast(replace(((COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0))  - floor(COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0))),'.','') as int)))*6  as numeric(36,2))		
				END As 'Economy',
				floor(COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0)) * 6 + cast(replace(((COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0))  - floor(COALESCE(sum(Overs),0) + COALESCE(PlayerPastRecords.TotalOvers,0))),'.','') as int) as 'TotalBalls',
			    (COALESCE(PlayerPastRecords.FiveWickets,0) + COALESCE(count(Case When Wickets >=5 Then 1 Else Null End),0)) As 'FiveWickets',
				(COALESCE(PlayerPastRecords.OnFieldCatch,0) + COALESCE(sum (Catches),0)) as 'OnFieldCatch',
			 	(COALESCE(PlayerPastRecords.OnFieldRunOut,0) + COALESCE(sum (RunOut),0)) as 'OnFieldRunOut',
				(COALESCE(PlayerPastRecords.OnFieldStump,0) + COALESCE(sum (Stump),0)) as 'OnFieldStump',
				COALESCE(count (case when Overs != null and Overs != 0 then 1 else null end),0) as 'TotalBowlingInnings',
				Players.[Name] AS 'PlayerName',
				--Teams.Id As 'TeamId',					
				--Teams.[Name] As 'TeamName',
				Case When Players.[FileName] is null then 'noImage.jpg' else Players.[FileName] end As 'FileName',
				Players.DOB AS 'DOB',
				--Convert(varchar(10), Players.DOB) as 'DOB',
				Players.Id As 'PlayerId'
	
		FROM Players
		left join PlayerPastRecords On Players.Id = PlayerPastRecords.PlayerId
		left join PlayerScores ON PlayerScores.PlayerId = Players.Id
		left join TeamPLayers ON TeamPLayers.PlayerId = PlayerScores.PlayerId
		left join Teams ON Teams.Id = TeamPLayers.TeamId
		left join Matches ON PlayerScores.MatchId = Matches.Id
	
		WHERE 
				Players.Id = @paramPlayerId And 
				(@paramTeamId is null or Teams.Id = @paramTeamId) And
				(@paramMatchTypeId is null or Matches.MatchTypeId = @paramMatchTypeId) And
				(@paramSeason is null or Matches.Season = @paramSeason)
		GROUP BY Players.Id,
				 Players.[Name],
                 Players.DOB,
				 Players.[FileName],
				 PlayerPastRecords.TotalMatch,
				 PlayerPastRecords.TotalInnings,
				 PlayerPastRecords.TotalBatRuns,
				 PlayerPastRecords.TotalBatBalls,
				 PlayerPastRecords.TotalFours,
				 PlayerPastRecords.TotalSixes,
				 PlayerPastRecords.TotalNotOut,
				 PlayerPastRecords.GetBowled,
				 PlayerPastRecords.GetCatch,
				 PlayerPastRecords.GetStump,
				 PlayerPastRecords.GetRunOut,
				 PlayerPastRecords.GetHitWicket,
				 PlayerPastRecords.GetLBW,
				 PlayerPastRecords.TotalFours,
				 PlayerPastRecords.TotalSixes,
				 PlayerPastRecords.TotalNotOut,
				 PlayerPastRecords.GetBowled,
				 PlayerPastRecords.GetCatch,
				 PlayerPastRecords.GetStump,
				 PlayerPastRecords.GetRunOut,
				 PlayerPastRecords.GetHitWicket,
				 PlayerPastRecords.GetLBW,
				 PlayerPastRecords.NumberOf50s,
				 PlayerPastRecords.NumberOf100s,
				 PlayerPastRecords.FiveWickets,
				 PlayerPastRecords.OnFieldCatch,
				 PlayerPastRecords.OnFieldRunOut,
				 PlayerPastRecords.OnFieldStump,
				 PlayerPastRecords.DoBowled,
				 PlayerPastRecords.DoCatch,
				 PlayerPastRecords.DoHitWicket,
				 PlayerPastRecords.DoLBW,
				 PlayerPastRecords.TotalOvers,
				 PlayerPastRecords.DoStump,
				 PlayerPastRecords.TotalBallRuns,
				 PlayerPastRecords.TotalWickets,
				 PlayerPastRecords.TotalMaidens,
				 PlayerPastRecords.BestScore
				 ) 
				 as SinglePlayerStatistics
END
GO 




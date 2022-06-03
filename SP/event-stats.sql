Alter PROCEDURE [usp_GetEventStatistics]
@paramEventId AS INT
AS Begin
select 
count(matches.Id) as 'Matches',
sum(PlayerScores.Four) as 'Fours',
sum(PlayerScores.Six) as 'Sixes',
sum(PlayerScores.RunOut) as 'RunOuts',
sum(PlayerScores.Stump) as 'Stumps',
sum(PlayerScores.Catches) as 'Catches',
sum(PlayerScores.Wickets) as 'Wickets',
eve.[Name] as 'Event',
eve.Organizor as 'Organizor',
eve.StartDate as 'StartDate',
eve.EndDate as 'EndDate',
eve.NumberOfGroup as 'Groups'
from matches
left join PlayerScores on PlayerScores.MatchId = Matches.Id
left join [Events] as eve on eve.id = Matches.EventId
where eventId = @paramEventId
group by 
eve.[name],
eve.StartDate,
eve.EndDate,
eve.NumberOfGroup,
eve.Organizor
End
Go

exec [usp_GetEventStatistics] 5
--group by PlayerScores.PlayerId

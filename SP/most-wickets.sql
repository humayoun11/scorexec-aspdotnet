Alter PROCEDURE [usp_GetMostWickets]
@paramTeamId AS INT,
@paramSeason As Int,
@paramOvers As Int,
@paramMatchType As Int,
@paramTournamentId As Int,
@paramPlayerRoleId As Int
AS
BEGIN
	SELECT  top 10
			count (PlayerScores.MatchId) as 'TotalMatch',
			count (case when Overs != null and Overs != 0 then 1 else null end) as 'TotalInnings',
			sum (Wickets) as 'MostWickets',
			Case When Players.ProfileUrl is null  then  'dummy.jpg' else Players.ProfileUrl end  AS 'Image',
			Players.[Name] AS 'PlayerName'
			
	
	FROM PlayerScores
	Inner join Players ON PlayerScores.PlayerId = Players.Id
	Inner join Matches ON PlayerScores.MatchId = Matches.Id
	left join [Events] On Matches.EventId = [Events].Id
	
	
	
	WHERE  
		  (@paramSeason IS NUll OR Matches.Season = @paramSeason)	And
		  (@paramTeamId IS NUll OR PlayerScores.TeamId = @paramTeamId )	And
		  (@paramOvers IS NUll OR Matches.MatchOvers = @paramOvers)	And 
		  (@paramMatchType IS NULL OR Matches.MatchTypeId = @paramMatchType) And 
		  (@paramPlayerRoleId IS NUll OR Players.PlayerRoleId = @paramPlayerRoleId) ANd
		  (@paramTournamentId IS NUll OR [Events].id = @paramTournamentId) ANd
		    (Players.IsDeactivated != 1) and 
			(Players.IsGuestorRegistered != 'Guest' or Players.IsGuestorRegistered is null)
	
	
	GROUP BY PlayerScores.PlayerId,
			Players.Name,
			Players.ProfileUrl
		--	PlayerScores.Bat_Runs

	order by sum(Wickets) desc ;
END
go
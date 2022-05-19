Alter PROCEDURE [usp_GetMostMaidens]
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
			sum (Maiden) as 'Maidens',
			Case When Players.[FileName] is null  then  'noImage.jpg' else Players.[FileName] end  AS 'Image',
			Players.[Name] AS 'PlayerName'
			
	
	FROM PlayerScores
	Inner join Players ON PlayerScores.PlayerId = Players.Id
	Inner join Matches ON PlayerScores.MatchId = Matches.Id
	left join [Events] On Matches.EventId = [Events].Id
	
	
	
	WHERE (@paramSeason IS NUll OR Matches.Season = @paramSeason)	And
		(@paramTeamId IS NUll OR PlayerScores.TeamId = @paramTeamId )	And
		  (@paramOvers IS NUll OR Matches.MatchOvers = @paramOvers)	And 
		  (@paramPlayerRoleId IS NUll OR Players.PlayerRoleId = @paramPlayerRoleId) And
		  (@paramMatchType IS NUll OR Matches.MatchTypeId = @paramMatchType) And
		  (@paramTournamentId IS NUll OR [Events].Id = @paramTournamentId) And
		    (Players.IsDeactivated != 1) and 
			(Players.IsGuestorRegistered != 'Guest' or Players.IsGuestorRegistered is null)
	
	GROUP BY PlayerScores.PlayerId,
			Players.[Name],
			Players.[FileName]
		--	PlayerScores.Bat_Runs

	order by sum(Maiden) desc ;
END
go
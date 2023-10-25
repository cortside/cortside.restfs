$tables = @("Subject", "Widget")

foreach ($table in $tables) {
	echo "exporting $table..."
	$sql = "SELECT * FROM RestFS.dbo.$table"
	$filename = "src\\Cortside.RestFS.WebApi.IntegrationTests\\SeedData\\$table.csv"
	Invoke-Sqlcmd -Query $sql -HostName localhost | Export-Csv -Path $filename -NoTypeInformation
}

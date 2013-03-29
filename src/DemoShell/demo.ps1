begin {
	"Boot-Script"
	Get-Location
	$x = 200
	"x = $x"
}
process {
	if ($_) {
		"Input = '$_'"
	} else {
		"Null Input"
	}
}
end {
	"kurz warten..."
	[Threading.Thread]::Sleep(2000);
	"fertig."
}
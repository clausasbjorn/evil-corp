Param
(
    [String]
    $Path,

    [String]
    $Version
)

$ServiceManifests = Get-ChildItem -Path $Path -Filter ServiceManifest.xml -Recurse | % { $_.FullName }
$ApplicationManifests = Get-ChildItem -Path $Path -Filter ApplicationManifest.xml -Recurse | % { $_.FullName }

ForEach ($Manifest in $ServiceManifests) {
  $Xml = [xml](Get-Content $Manifest)

  $Xml.ServiceManifest.Version = $Version
  $Xml.ServiceManifest.CodePackage.Version = $Version
  $Xml.ServiceManifest.ConfigPackage.Version = $Version

  $Xml.Save($Manifest)
}

ForEach ($Manifest in $ApplicationManifests) {
  $Xml = [xml](Get-Content $Manifest)
  $Xml.ApplicationManifest.ApplicationTypeVersion = $Version
  ForEach ($Node in $Xml.ApplicationManifest.ServiceManifestImport) {
    $Ref = $Node.ServiceManifestRef
    If ($Ref) {
      $Ref.ServiceManifestVersion = $Version
    }
  }
  $Xml.Save($Manifest)
}

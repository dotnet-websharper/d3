param ([string]$env = "local")

# pushes build/html to gh-pages branch
$msg = 'publishGitHubPages: build/html -> gh-pages'
$gitURL = "https://github.com/intellifactory/websharper.d3.git"

write-host -foregroundColor "green" "=====> $msg"


function clearDir() {
  rm -r build/gh-pages -errorAction ignore
}

if ($env -eq "appveyor") {
  clearDir
  git clone $gitURL build/gh-pages 2>git.log
  cd build/gh-pages
  git config credential.helper "store --file=.git/credentials"
  echo ("https://$GH_TOKEN" + ":@github.com") > .git/credentials	
  git config user.name "AppVeyor"
  git config user.email "websharper-support@intellifactory.com"
} else {
  clearDir
  cd build
  git clone .. gh-pages 2>git.log
  cd gh-pages
}

git checkout gh-pages 2>git.log
cp -r -force ../html/* .
git add . 2>git.log
git commit -am $msg 2>git.log
git push origin gh-pages 2>git.log
cd ../..
clearDir
write-host -foregroundColor "green" "=====> DONE"

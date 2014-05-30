# pushes build/html to gh-pages branch
$msg = 'publishGitHubPages: build/html -> gh-pages'
$gitURL = "https://github.com/intellifactory/websharper.d3.git"
$inAV = false
if ($env:APPVEYOR_BUILD_NUMBER) { $inAV = true; }

write-host -foregroundColor "green" "=====> $msg"

function clearDir() {
  rm -r build/gh-pages -errorAction ignore
}

if ($inAV) {
  git config credential.helper "store --file=.git/credentials"
  echo ("https://$GH_TOKEN" + ":@github.com") > .git/credentials	
  git config user.name "AppVeyor"
  git config user.email "websharper-support@intellifactory.com"
  clearDir
  git clone $gitURL build/gh-pages  
  cd build/gh-pages
} else {
  clearDir
  cd build
  git clone .. gh-pages
  cd gh-pages
}

git checkout gh-pages
cp -r -force ../html/* .
git add .
git commit -am $msg
git push origin gh-pages
cd ../..
clearDir
write-host -foregroundColor "green" "=====> DONE"

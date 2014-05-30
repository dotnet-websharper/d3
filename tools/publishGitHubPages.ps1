# pushes build/html to gh-pages branch
$msg = 'publishGitHubPages: build/html -> gh-pages'
write-host -foregroundColor "green" "=====> $msg"
rm -r build/gh-pages -errorAction ignore
$d = mkdir -force build
cd build
git clone .. gh-pages
cd gh-pages
git checkout gh-pages
cp -r -force ../../build/html/* .
git add .
git commit -am $msg
git push origin gh-pages
cd ../..
rm -r build/gh-pages -errorAction ignore
write-host -foregroundColor "green" "=====> DONE"

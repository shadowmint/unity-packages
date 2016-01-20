# Usage: retag.sh unity-n-ui 0.0.1
cd $1 && git tag -d $2 && git push origin :refs/tags/$2 && git tag $2 && git push --tags

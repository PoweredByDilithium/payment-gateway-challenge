#!/usr/bin/env sh
. "$(dirname -- "$0")/_/husky.sh"

dotnet test --no-build

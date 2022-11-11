#!/bin/sh

# If global.json is present, .NET will try to use it.
# However, the version we use is unavailable on AppCenter.
# Given that global.json is for MAUI at the momment,
# we work around the issue by removing it.

pwd
rm ../../global.json

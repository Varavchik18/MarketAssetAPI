#!/bin/bash
set -e

echo "Checking installed dotnet tools..."
dotnet tool list -g

echo "Listing directories in the current directory..."
ls -d */
echo "Listing files in the current directory..."
ls -l
echo "Current location ... "
pwd

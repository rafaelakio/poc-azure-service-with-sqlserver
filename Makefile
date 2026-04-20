.PHONY: install build test lint format clean

install:
	dotnet restore

build:
	dotnet build --configuration Release

test:
	dotnet test --configuration Release

lint:
	dotnet format --verify-no-changes

format:
	dotnet format

clean:
	dotnet clean
	find . -type d -name bin -prune -exec rm -rf {} + || true
	find . -type d -name obj -prune -exec rm -rf {} + || true

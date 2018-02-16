UNITY_APP="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

build:
	@echo "Building project..."
	@$(UNITY_APP) -batchmode -runEditorTests -logFile "Builds/build.log" -quit -executeMethod ProjectBuilder.BuildProject
	@echo "Project built!"

clean:
	rm -fR Builds/*

.PHONY: build clean

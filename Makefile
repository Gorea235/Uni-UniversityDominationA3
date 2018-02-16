UNITY_APP="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

build:
	$(UNITY_APP) -batchmode -runEditorTests -logFile "Builds/build.log" -quit -executeMethod ProjectBuilder.BuildProject

clean:
	rm -fR Builds/*

.PHONY: build clean

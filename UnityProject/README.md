# UnityProject

Unity project files live here.

Current project:

- `Understory_Unity_Proof`
- Unity `6000.5.2f1`
- Universal Render Pipeline `17.5.0`
- iOS Build Support installed
- starter scene:
  `Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`

Run the verifier from repo root with:

```sh
"/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$(pwd)/UnityProject/Understory_Unity_Proof" -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady -logFile "$(pwd)/Reports/unity-verify-ready.log"
```

Do not start production systems beyond Scene 01 until the visual proof lands.

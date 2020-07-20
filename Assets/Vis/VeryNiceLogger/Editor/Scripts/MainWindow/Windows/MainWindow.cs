using UnityEditor;
using Vis.Utils;

public class MainWindow : EditorWindow
{
    private const string _dbPointerName = "VeryNiceLoggerDbPointer";
    private const string _configName = "VeryNiceLoggerConfig.asset";

    private static MainWindow _intance;

    [MenuItem("Vis/Very Nice Logger Options")]
    private static void windowInitializer() => _intance = GetWindow<MainWindow>();

    private Config _config
    {
        get
        {
            if (__config == default)
                __config = AssetsUtils.GetScriptableObjectFromDB<Config>(_dbPointerName, _configName);
            return __config;
        }
    }
    private Config __config;

    private ConfigView _configView = new ConfigView();
    private TestView _testView = new TestView();

    private void OnGUI()
    {
        _configView.Render(_config);
        _testView.Render(_config);
    }
}

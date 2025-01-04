using CSharpToJsonSchema;
using System.Text;
using WinFormsApp2;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Ollama.IntegrationTests;

public enum ErrorType
{
    AutomationTool,
    TeamCenter,
    iPLM,
    CommonError
}

public class ModelInfo
{
    public string fileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[GenerateJsonSchema]
public interface ICADFunctions
{
    [Description("create a  part or assembly file in creo.")]
    public ModelInfo CreateFileinCreo(
        [Description("Name of the file")] string PartName,
        [Description("File type : part or assembly")] string FileType );

    [Description("Gets current model information from creo.")]
    public string GetModelInfo();

    [Description("Gets feature count in model")]
    public int GetFeatureCount();

    [Description("Gets count of child parts in model")]
    public int GetChildCount();

    [Description("Highlight curved edges in model")]
    public int HighlightallCurve();



    /*    [Description("When user is facing error in CAD not in iPLM,TeamCenter or AutomationTool")]
        public Weather GetErrorInfoCommon(
      [Description("This is process in which user is faing error")] string toolName,
      ErrorType error = ErrorType.CommonError);
    */
    /*    [Description("When user is facing error in TeamCenter")]
        public Task<Weather> GetCurrentWeatherAsync(
            [Description("The city and state, e.g. San Francisco, CA")] string location,
             ErrorType error = ErrorType.AutomationTool,
            CancellationToken cancellationToken = default);*/
}

public class WeatherService : ICADFunctions
{
    public ModelInfo CreateFileinCreo(string PartNumber,string FileType)
    {
        ModelInfo info = new ModelInfo
        {
            fileName = PartNumber,
            FileType = FileType,
            Description = "File Created in creo Successfully"
        };

        Creo.CreateFile("Test", "part");

        return info;
    }

    public string GetModelInfo()
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder s3 = new StringBuilder();
        Creo.GetInfo(sb,sb2,s3);

        new ModelInfo
        {
            fileName = sb.ToString(),
            FileType = sb2.ToString(),
            Description = $" {sb2.ToString()} filed named {sb.ToString()} is opened in current creo session. "
        };
        return $" {sb2.ToString()} filed named {sb.ToString()} is opened in current creo session. ";
    }

    public int GetFeatureCount()
    { 
        return 10;
    }

    public int GetChildCount()
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder s3 = new StringBuilder();
        Creo.GetInfo(sb, sb2, s3);

        int feats = Convert.ToInt32(s3.ToString());
        return feats; 
    }
    public int HighlightallCurve()
    {
        Creo.HighLightallCurveEdges(); 
        return 10;
    }
    //public ErrorInfo GetErrorInfoAutomation(string location, ErrorType error = ErrorType.AutomationTool)
    //{
    //    return new ErrorInfo
    //    {
    //        toolName = location,
    //        CriticalScore = 22.0,
    //        errorType = error,
    //        Description = $"The tool {location} start up files are misssing. Ask user to copy files from CADTools sharefolder to local machine and then restart creo.",
    //    };
    //}

    //public ErrorInfo GetTeamCenterInfo(string location, ErrorType error = ErrorType.AutomationTool)
    //{
    //    return new ErrorInfo
    //    {
    //        toolName = location,
    //        CriticalScore = 22.0,
    //        errorType = error,
    //        Description = "The error is in cache. ask user to clear the cache and then save it in tc",
    //    };
    //}
    //public ErrorInfo GetErrorInfoiPLM(string location, ErrorType error = ErrorType.AutomationTool)
    //{
    //    return new ErrorInfo
    //    {
    //        toolName = location,
    //        CriticalScore = 22.0,
    //        errorType = error,
    //        Description = "The iPLM server is down plz try after some time.",
    //    };
    //}
    /*
        public Weather GetErrorInfoCommon(string location, ErrorType error = ErrorType.AutomationTool)
        {
            return new Weather
            {
                toolName = location,
                CriticalScore = 22.0,
                errorType = error,
                Description = "PLz rise a ticket for resolving this error",
            };
        }*/

    /*    public Task<Weather> GetCurrentWeatherAsync(string location, ErrorType error = ErrorType.AutomationTool, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Weather
            {
                toolName = "SSP",
                CriticalScore = 22.0,
                errorType = error,
                Description = "The zx file is Missing",
            });
        }*/
}
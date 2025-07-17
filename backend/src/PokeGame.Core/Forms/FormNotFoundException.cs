using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Forms;

public class FormNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified form was not found.";

  public string Form
  {
    get => (string)Data[nameof(Form)]!;
    private set => Data[nameof(Form)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Form)] = Form;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public FormNotFoundException(string form, string propertyName) : base(BuildMessage(form, propertyName))
  {
    Form = form;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string form, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Form), form)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

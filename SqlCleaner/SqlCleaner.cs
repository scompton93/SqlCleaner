using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlCleaner
{
    public partial class SqlCleaner : Form
    {
        public SqlCleaner()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(
                System.Reflection.Assembly.GetExecutingAssembly().Location
            );
        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                TSqlParser parser = new TSql120Parser(true);
                IList<ParseError> parseErrors;
                TSqlFragment sqlFragment = parser.Parse(
                    new StringReader(inputText.Text),
                    out parseErrors
                );

                TSqlScript script = (TSqlScript)sqlFragment;
                foreach (var batch in script.Batches)
                {
                    foreach (
                        var stmt in batch.Statements.OfType<Microsoft.SqlServer.TransactSql.ScriptDom.ExecuteStatement>()
                    )
                    {
                        var paramterers = stmt.ExecuteSpecification.ExecutableEntity.Parameters;

                        var paramaterHeader = ((StringLiteral)paramterers[1].ParameterValue).Value;
                        var queryHeader = ((StringLiteral)paramterers[2].ParameterValue).Value;

                        var x = paramterers[3].ParameterValue.GetType().ToString();

                        var paramsArray = paramaterHeader.Split(',');

                        for (int i = 0; i < paramsArray.Length; i++)
                        {
                            var paramOffset = i + 3;
                            if (paramterers[paramOffset].ParameterValue is StringLiteral)
                            {
                                paramsArray[i] =
                                    paramsArray[i]
                                    + string.Format(
                                        " = '{0}',",
                                        (
                                            (StringLiteral)paramterers[paramOffset].ParameterValue
                                        ).Value
                                    );
                            }
                            else if (paramterers[paramOffset].ParameterValue is IntegerLiteral)
                            {
                                paramsArray[i] =
                                    paramsArray[i]
                                    + string.Format(
                                        " = {0},",
                                        (
                                            (IntegerLiteral)paramterers[paramOffset].ParameterValue
                                        ).Value
                                    );
                            }
                            else if (paramterers[paramOffset].ParameterValue is NumericLiteral)
                            {
                                paramsArray[i] =
                                    paramsArray[i]
                                    + string.Format(
                                        " = {0},",
                                        (
                                            (NumericLiteral)paramterers[paramOffset].ParameterValue
                                        ).Value
                                    );
                            }
                        }

                        var declareStmt =
                            "DECLARE " + string.Join(Environment.NewLine, paramsArray);
                        declareStmt = declareStmt.Remove(declareStmt.Length - 1) + ";";
                        sql = declareStmt + Environment.NewLine + queryHeader;
                    }
                }
                inputText.Text = sql;
            }
            catch
            {
                inputText.Text = "Error in query";
            }
        }

        private void copyBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(inputText.Text);
        }
    }
}

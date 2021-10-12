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
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            TSqlParser parser = new TSql120Parser(true);
            IList<ParseError> parseErrors;
            TSqlFragment sqlFragment = parser.Parse(new StringReader(inputText.Text), out parseErrors);

            TSqlScript script = (TSqlScript)sqlFragment;
            var batch = script.Batches[0];

            ExecuteStatement stmt = batch.Statements.OfType<Microsoft.SqlServer.TransactSql.ScriptDom.ExecuteStatement>().First();

            var parameters = stmt.ExecuteSpecification.ExecutableEntity.Parameters;

            var parameterHeader = ((StringLiteral)parameters[1].ParameterValue).Value;
            var queryHeader = ((StringLiteral)parameters[2].ParameterValue).Value;

            var paramsArray = parameterHeader.Split(',');

            for (int i = 0; i < paramsArray.Length; i++)
            {
                var paramOffset = i + 3;
                if (parameters[paramOffset].ParameterValue.GetType().ToString() == "Microsoft.SqlServer.TransactSql.ScriptDom.StringLiteral")
                {
                    paramsArray[i] = paramsArray[i] + string.Format(" = '{0}',", ((StringLiteral)parameters[paramOffset].ParameterValue).Value);
                }
                else if (parameters[paramOffset].ParameterValue.GetType().ToString() == "Microsoft.SqlServer.TransactSql.ScriptDom.IntegerLiteral")
                {
                    paramsArray[i] = paramsArray[i] + string.Format(" = {0},", ((IntegerLiteral)parameters[paramOffset].ParameterValue).Value);
                }
                else if (parameters[paramOffset].ParameterValue.GetType().ToString() == "Microsoft.SqlServer.TransactSql.ScriptDom.NumericLiteral")
                {
                    paramsArray[i] = paramsArray[i] + string.Format(" = {0},", ((NumericLiteral)parameters[paramOffset].ParameterValue).Value);
                }
            }

            var declareStmt = "DECLARE " + string.Join("\r\n\t", paramsArray);
            declareStmt = declareStmt.Remove(declareStmt.Length - 1) + ";";
            var finalQuery = declareStmt + "\r\n\r\n" + queryHeader;

            inputText.Text = finalQuery;

        }

        private void copyBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(inputText.Text);
        }
    }
}

using System.Diagnostics;
using System.Text.RegularExpressions;

internal class Program
{
    private enum Category
    {
        None,
        x,
        m,
        a,
        s
    };

    private enum Op
    {
        None,
        Greater,
        Less
    };

    record DataRecord(int x, int m, int a, int s);
    record Condition(Category Category, Op Op, int Value);

    private class Workflow
    {
        public List<Tuple<Condition, string>> ConditionTargetPairs = [];
        public string LastTarget = string.Empty;
    }

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;
        bool isWorkflow = true;

        List<string> rawWorkflows = [];
        List<string> rawRecords = [];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                isWorkflow = false;
                continue;
            }

            if (isWorkflow)
            {
                rawWorkflows.Add(line);
            }
            else
            {
                rawRecords.Add(line);
            }
        }

        Dictionary<string, Workflow> dict_label_workflows = ParseWorkflowRules(rawWorkflows);
        List<DataRecord> records = ParseDataRecords(rawRecords);

        foreach (DataRecord record in records)
        {
            result += GetValidPartSum(record, dict_label_workflows);
        }
        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int GetValidPartSum(DataRecord record, Dictionary<string, Workflow> dict_label_workflows)
    {
        Workflow curWorkflow = dict_label_workflows["in"];
        string curWorkflowTarget = GetWorkflowTarget(record, curWorkflow);

        while (curWorkflowTarget != "A" && curWorkflowTarget != "R")
        {
            curWorkflow = dict_label_workflows[curWorkflowTarget];
            curWorkflowTarget = GetWorkflowTarget(record, curWorkflow);
        }

        // the record is accepted
        if (curWorkflowTarget == "A")
        {
            return record.a + record.m + record.s + record.x;
        }

        return 0;
    }

    private static string GetWorkflowTarget(DataRecord record, Workflow curWorkflow)
    {
        string workflowTarget = string.Empty;

        List<Tuple<Condition, string>> condition_target_pairs = curWorkflow.ConditionTargetPairs;
        for (int c = 0; c < condition_target_pairs.Count; c++)
        {
            Tuple<Condition, string> condition_target = condition_target_pairs[c];
            Condition condition = condition_target.Item1;
            string target = condition_target.Item2;

            if (IsConditionPassed(record, condition))
            {
                workflowTarget = target;
                break;
            }
        }

        if (string.IsNullOrEmpty(workflowTarget))
        {
            workflowTarget = curWorkflow.LastTarget;
        }

        return workflowTarget;
    }

    private static bool IsConditionPassed(DataRecord record, Condition condition)
    {
        int recordValue = GetRecordValue(record, condition.Category);

        bool isConditionPassed = IsPassed(recordValue, condition.Op, condition.Value);

        return isConditionPassed;
    }

    private static bool IsPassed(int recordValue, Op op, int conditionValue)
    {
        switch (op)
        {
            case Op.Greater:
                return recordValue > conditionValue;

            case Op.Less:
                return recordValue < conditionValue;

            case Op.None:

            default:
                throw new Exception();
        }
    }

    private static int GetRecordValue(DataRecord record, Category category)
    {
        switch (category)
        {
            case Category.x:
                return record.x;

            case Category.m:
                return record.m;

            case Category.a:
                return record.a;

            case Category.s:
                return record.s;

            case Category.None:
            default:
                throw new Exception();
        }
    }

    private static Dictionary<string, Workflow> ParseWorkflowRules(List<string> rawWorkflows)
    {
        Dictionary<string, Workflow> dict_label_workflows = [];
        char[] separator = ['{', '}'];

        foreach (string line in rawWorkflows)
        {
            string[] tmp = line.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string label = tmp[0];

            Workflow workflow = ParseWorkflow(tmp[1]);
            dict_label_workflows.Add(label, workflow);
        }

        return dict_label_workflows;
    }

    private static Workflow ParseWorkflow(string rawWorkflow)
    {
        Workflow workflow = new();
        string[] raw_condition_targets = rawWorkflow.Split(',', StringSplitOptions.TrimEntries).ToArray();
        workflow.LastTarget = raw_condition_targets.Last();

        List<Tuple<Condition, string>> tuples = [];
        List<Condition> conditions = new(raw_condition_targets.Length - 1);
        for (int i = 0; i < raw_condition_targets.Length - 1; i++)
        {
            string[] rawPair = raw_condition_targets[i].Split(':').ToArray();
            string target = rawPair[1];

            Condition condition = ParseCondition(rawPair[0]);

            tuples.Add(new Tuple<Condition, string>(condition, target));
        }

        workflow.ConditionTargetPairs = tuples;

        return workflow;
    }

    private static Condition ParseCondition(string rawCondition)
    {
        Category category = ParseCategory(rawCondition[0]);
        Op op = ParseOperation(rawCondition[1]);
        int value = int.Parse(rawCondition.Substring(2));

        return new Condition(category, op, value);
    }

    private static Op ParseOperation(char v)
    {
        switch (v)
        {
            case '>':
                return Op.Greater;

            case '<':
                return Op.Less;

            default:
                break;
        }
        return Op.None;
    }

    private static Category ParseCategory(char v)
    {
        switch (v)
        {
            case 'x':
                return Category.x;

            case 'm':
                return Category.m;

            case 'a':
                return Category.a;

            case 's':
                return Category.s;

            default:
                break;
        }
        return Category.None;
    }

    private static List<DataRecord> ParseDataRecords(List<string> dataStrings)
    {
        List<DataRecord> records = new(dataStrings.Count);
        Regex regex = new(@"x=(\d+),m=(\d+),a=(\d+),s=(\d+)");

        foreach (string data in dataStrings)
        {
            Match match = regex.Match(data);
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int m = int.Parse(match.Groups[2].Value);
                int a = int.Parse(match.Groups[3].Value);
                int s = int.Parse(match.Groups[4].Value);

                records.Add(new DataRecord(x, m, a, s));
            }
        }

        return records;
    }
}
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
        GT,
        LT,
    };

    record DataRecord(int x, int m, int a, int s);
    record Condition(Category Category, Op Op, int Value);
    record ValidRange(int valueStart, int valueEnd);

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

        Dictionary<string, Workflow> dict_label_workflows;
        List<DataRecord> records;
        GetData(lines, out dict_label_workflows, out records);

        long result = GetCombinationSum(dict_label_workflows);

        //foreach (DataRecord record in records)
        //{
        //    result += GetValidPartSum(record, dict_label_workflows);
        //}
        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long GetCombinationSum(Dictionary<string, Workflow> dict_label_workflows)
    {
        // ** step
        // accepted condition-paths
        List<List<Condition>> List_acceptedConditions = [];

        Queue<Tuple<Workflow, List<Condition>>> que_workflows = [];
        que_workflows.Enqueue(new Tuple<Workflow, List<Condition>>(dict_label_workflows["in"], []));

        // traverse the workflows for possible condition-paths
        while (que_workflows.Count > 0)
        {
            // current workflow
            Tuple<Workflow, List<Condition>> queItem = que_workflows.Dequeue();
            Workflow curWFlow = queItem.Item1;
            List<Condition> curWF_acceptedConditions = queItem.Item2;

            List<Tuple<Condition, string>> curWF_conditionTargetPairs = curWFlow.ConditionTargetPairs;

            // go through each condition
            for (int con = 0; con < curWF_conditionTargetPairs.Count; con++)
            {
                Tuple<Condition, string> condition_target = curWF_conditionTargetPairs[con];
                Condition curWF_condition = condition_target.Item1;
                string curWF_Target = condition_target.Item2;

                // pass the condition
                {
                    if (curWF_Target != "R")
                    {
                        // "A"
                        if (curWF_Target == "A")
                        {
                            // no conflict with existing condition-path
                            if (IsNoneConflict(curWF_condition, curWF_acceptedConditions))
                            {
                                List<Condition> tmp = curWF_acceptedConditions.ToList();
                                tmp.Add(curWF_condition);

                                List_acceptedConditions.Add(tmp);
                            }
                        }
                        // next workflow
                        else
                        {
                            Workflow nextWorkflow = dict_label_workflows[curWF_Target];

                            List<Condition> nextWF_acceptedConditions = curWF_acceptedConditions.ToList();
                            nextWF_acceptedConditions.Add(curWF_condition);

                            que_workflows.Enqueue(new Tuple<Workflow, List<Condition>>(nextWorkflow, nextWF_acceptedConditions));
                        }
                    }
                    // "R"
                    else
                    {
                        // do nothing;
                        //ignore the condition-path
                    }
                }

                // not pass the condition
                {
                    Condition reverseCondition = GetReverseCondition(curWF_condition);

                    // if this is the last condition, check the last target
                    if (con == curWF_conditionTargetPairs.Count - 1)
                    {
                        //if it is A
                        if (curWFlow.LastTarget == "A")
                        {
                            if (IsNoneConflict(curWF_condition, curWF_acceptedConditions))
                            {
                                List<Condition> tmp = curWF_acceptedConditions.ToList();
                                tmp.Add(reverseCondition);

                                List_acceptedConditions.Add(tmp);
                            }
                        }
                        // if R
                        else if (curWFlow.LastTarget == "R")
                        {
                            // do nothing
                        }
                        // if next flow
                        else //if (curWFlow.LastTarget != "R")
                        {
                            Workflow nextWorkflow = dict_label_workflows[curWFlow.LastTarget];
                            List<Condition> nextWF_acceptedConditions = curWF_acceptedConditions.ToList();
                            nextWF_acceptedConditions.Add(reverseCondition);

                            que_workflows.Enqueue(new Tuple<Workflow, List<Condition>>(nextWorkflow, nextWF_acceptedConditions));
                        }
                    }
                    else
                    {
                        // next condition
                        curWF_acceptedConditions.Add(reverseCondition);
                    }
                }
            }
        }

        // ** step
        long result = 0;
        foreach (List<Condition> conditions in List_acceptedConditions)
        {
            // get merged conditions
            Dictionary<Category, ValidRange> dict_category_range = GetMergedConditions(conditions);

            // get combinations
            long count_combinations = GetValidCombinations(dict_category_range);

            result += count_combinations;
        }

        //233352255800000
        //167409079868000
        //167409079868000
        return result;
    }

    private static long GetValidCombinations(Dictionary<Category, ValidRange> dict_category_range)
    {
        long result = 1;

        foreach (ValidRange range in dict_category_range.Values)
        {
            if (range.valueEnd - range.valueStart > 1)
            {
                result *= (range.valueEnd - range.valueStart - 1);
            }
            else
            {
                Console.WriteLine("skip");
            }
        }

        return result;
    }

    private static bool IsNoneConflict(Condition condition, List<Condition> existingConditions)
    {
        foreach (Condition acceptedCondition in existingConditions)
        {
            if (IsConflict(condition, acceptedCondition))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsConflict(Condition condition1, Condition condition2)
    {
        if (condition1.Category != condition2.Category)
        {
            return false;
        }

        if (condition1.Op == condition2.Op)
        {
            return false;
        }

        // diff Op, check values
        if (condition1.Value == condition2.Value)
        {
            return true;
        }

        int upperBound = int.MinValue;
        int lowerBound = int.MaxValue;

        List<Condition> conditions = [condition1, condition2];
        foreach (Condition condition in conditions)
        {
            if (condition.Op == Op.GT)
            {
                lowerBound = condition.Value;
            }
            else if (condition.Op == Op.LT)
            {
                upperBound = condition.Value;
            }
            else
            {
                throw new Exception();
            }
        }

        if (lowerBound >= upperBound)
        {
            return true;
        }

        return false;
    }

    private static List<ValidRange> GetValueRanges(List<Condition> conditions)
    {
        HashSet<Category> categories = [];
        List<ValidRange> validRanges = [];

        Dictionary<Category, ValidRange> dict_category_range = GetMergedConditions(conditions);

        foreach (Condition condition in conditions)
        {
        }

        return validRanges;
    }

    private static Dictionary<Category, ValidRange> GetMergedConditions(List<Condition> conditions)
    {
        Dictionary<Category, ValidRange> dict_category_Range = [];

        // init
        foreach (Category cat in Enum.GetValues(typeof(Category)))
        {
            if (cat != Category.None)
            {
                dict_category_Range.Add(cat, new ValidRange(0, 4001));
            }
        }

        foreach (Condition con in conditions)
        {
            ValidRange oldRange = dict_category_Range[con.Category];

            if (con.Op == Op.GT)
            {
                int start = int.Max(dict_category_Range[con.Category].valueStart, con.Value);
                ValidRange newRange = oldRange with { valueStart = start };

                dict_category_Range[con.Category] = newRange;
            }
            else if (con.Op == Op.LT)
            {
                int end = int.Min(dict_category_Range[con.Category].valueEnd, con.Value);
                ValidRange newRange = oldRange with { valueEnd = end };

                dict_category_Range[con.Category] = newRange;
            }
            else
            {
                throw new Exception();
            }
        }

        return dict_category_Range;
    }

    private static Condition GetReverseCondition(Condition condition)
    {
        Op rvOp = condition.Op == Op.GT ? Op.LT : Op.GT;
        int rvValue = condition.Op == Op.GT ? condition.Value + 1 : condition.Value - 1;
        return new Condition(condition.Category, rvOp, rvValue);
    }

    private static void GetData(string[] lines, out Dictionary<string, Workflow> dict_label_workflows, out List<DataRecord> records)
    {
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

        dict_label_workflows = ParseWorkflowRules(rawWorkflows);
        records = ParseDataRecords(rawRecords);
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
            case Op.GT:
                return recordValue > conditionValue;

            case Op.LT:
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

        foreach (string line in rawWorkflows)
        {
            string[] tmp = line.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
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
                return Op.GT;

            case '<':
                return Op.LT;

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
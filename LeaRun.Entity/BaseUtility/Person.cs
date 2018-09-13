using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeaRun.Entity.BaseUtility
{
    public class Person
    {
        public int PId { get; set; }
        public string PName { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public int UnitSortCode { get; set; }
        public Person()
        {
            TaskTypeList = new List<TaskType>();
            for (int i = 1; i <= 14; i++)
            {
                TaskType t = new TaskType();
                t.TaskTypeId = i;
                t.TaskTypeName = GetTaskTypeName(i);
                TaskTypeList.Add(t);
            }
        }
        public List<TaskType> TaskTypeList { get; set; }

        private string GetTaskTypeName(int tasktypeId)
        {
            if (tasktypeId == 1) return "保护犯罪现场";
            if (tasktypeId == 2) return "执行传唤";
            if (tasktypeId == 3) return "执行拘传";
            if (tasktypeId == 4) return "协助执行指定居所监视居住";
            if (tasktypeId == 5) return "协助执行拘留、逮捕";
            if (tasktypeId == 6) return "参与追捕在逃或者脱逃的犯罪嫌疑人";
            if (tasktypeId == 7) return "参与搜查任务";
            if (tasktypeId == 8) return "提押犯罪嫌疑人被告人或罪犯";
            if (tasktypeId == 9) return "看管犯罪嫌疑人被告人或罪犯";
            if (tasktypeId == 10) return "送达法律文书";
            if (tasktypeId == 11) return "保护检察人员安全";
            if (tasktypeId == 12) return "办公、办案、控申接待场所执勤";
            if (tasktypeId == 13) return "参与处置突发事件任务";
            if (tasktypeId == 14) return "完成其他任务";
            return "";
        }
    }
    //tasktype_id

    public class TaskType
    {
        public int TaskTypeId { get; set; }
        public string TaskTypeName { get; set; }
        public int Num { get; set; }
    }
}

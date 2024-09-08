using next.processor.api.interfaces;
using next.processor.api.models;

namespace next.processor.api.backing
{
    public class QueueProcessBegin(IApiWrapper wrapper) : BaseQueueProcess(wrapper)
    {
        public override int Index => -1;

        public override string Name => "Fetch";

        public override bool IsSuccess { get; protected set; }
        public override bool AllowIterateNext { get; protected set; }

        public override async Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record)
        {
            var data = await apiWrapper.FetchAsync();
            if (data == null) return null;
            IsSuccess = true;
            AllowIterateNext = data.Count > 0;
            if (data.Count > 1) data.Sort((a, b) => Guid.NewGuid().ToString().CompareTo(Guid.NewGuid().ToString()));
            return new(data);
        }
    }
}

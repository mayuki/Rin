namespace Rin.Core
{
    public class BodyDataTransformerSet
    {
        public IBodyDataTransformer Request { get; }
        public IBodyDataTransformer Response { get; }

        public BodyDataTransformerSet(IBodyDataTransformer request, IBodyDataTransformer response)
        {
            Request = request;
            Response = response;
        }
    }
}

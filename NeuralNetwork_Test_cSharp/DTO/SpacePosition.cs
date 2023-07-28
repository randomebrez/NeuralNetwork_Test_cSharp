using System.Text;

namespace NeuralNetwork_Test_cSharp.DTO
{
    public class SpacePosition
    {
        public SpacePosition(float[] coordinates)
        {
            _coordinates = coordinates;
        }

        private int _dimension => _coordinates.Length;

        private float[] _coordinates;

        public float GetCoordinate(int dimensionIndex)
        {
            if (dimensionIndex > _dimension)
                throw new Exception($"Max dimension id is {_dimension}. Requested {dimensionIndex}");
            return _coordinates[dimensionIndex];
        }

        public void SetCoordinate(int dimensionIndex, float value)
        {
            if (dimensionIndex > _dimension)
                throw new Exception($"Max dimension id is {_dimension}. Requested {dimensionIndex}");
            _coordinates[dimensionIndex] = value;
        }

        public void SetCoordinates(float[] values)
        {
            if (values.Length != _dimension)
                throw new Exception($"Parameter length should be equal to dimension : {_dimension}. Here {values.Length}");
            _coordinates = values;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var coordinate in _coordinates)
                result.Append($"{coordinate};");

            return result.ToString();
        }
    }
}

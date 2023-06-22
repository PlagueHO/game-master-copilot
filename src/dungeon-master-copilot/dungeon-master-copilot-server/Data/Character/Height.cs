namespace dungeon_master_copilot_server.Data.Character
{
    public class Height
    {
        /// <summary>
        /// The height value in meters
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Returns the height in inches, calculated from the <see cref="Value"/> property.
        /// </summary>
        /// <returns>The height in inches</returns>
        public double GetInches()
        {
            return Value * 39.37d;
        }

        /// <summary>
        /// Returns the height in feet and inches, calculated from the <see cref="Value"/> property.
        /// </summary>
        /// <returns>The height in feet and inches (e.g. "6' 2\"")</returns>
        public string GetFeetAndInches()
        {
            int feet = (int)(Value * 3.281d);
            int inches = (int)(Value * 39.37d % 12);
            return $"{feet}' {inches}\"";
        }

        /// <summary>
        /// Returns a string representation of the height measurement in meters.
        /// </summary>
        /// <returns>A string representation of the height measurement in meters (e.g. "1.82m")</returns>
        public override string ToString()
        {
            return $"{Value}m";
        }
    }
}

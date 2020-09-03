namespace TH.Core.Base.Database.Information
{
    /// <summary> MySQL Information: Datatype. </summary>
    internal enum DataType
    {
        // Integer
        TINYINT,
        SMALLINT,
        MEDIUMINT,
        INT,
        BIGINT,
        BIT,

        // Real
        FLOAT,
        DOUBLE,
        DECIMAL,

        // Text
        CHAR,
        VARCHAR,
        TINYTEXT,
        TEXT,
        MEDIUMTEXT,
        LONGTEXT,

        // Binary
        BINARY,
        VARBINARY,
        TINYBLOB,
        BLOB,
        MEDIUMBLOB,
        LONGBLOB,

        // Temporal
        DATE,
        TIME,
        YEAR,
        DATETIME,
        TIMESTAMP,

        // Spatial (geometry)
        POINT,
        LINESTRING,
        POLYGON,
        GEOMETRY,
        MULTIPOINT,
        MULTILINESTRING,
        MULTIPOLYGON,
        GEOMETRYCOLLECTION,

        // Other
        ENUM,
        SET
    }
}

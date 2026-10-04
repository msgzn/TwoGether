using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TwoGether.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutesAndMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "driver_routes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    destination = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    path = table.Column<LineString>(type: "geometry(LineString, 4326)", nullable: false),
                    distance_meters = table.Column<double>(type: "double precision", nullable: false),
                    duration_seconds = table.Column<double>(type: "double precision", nullable: false),
                    days = table.Column<int>(type: "integer", nullable: false),
                    departure_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    seats = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    inserted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_routes", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_routes_users_driver_id",
                        column: x => x.driver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_driver_routes_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "passenger_routes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    destination = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    days = table.Column<int>(type: "integer", nullable: false),
                    earliest_pickup = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    latest_pickup = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    inserted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passenger_routes", x => x.id);
                    table.ForeignKey(
                        name: "fk_passenger_routes_users_passenger_id",
                        column: x => x.passenger_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "route_matches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    common_days = table.Column<int>(type: "integer", nullable: false),
                    pickup_point = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    dropoff_point = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    pickup_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    walk_to_pickup_meters = table.Column<double>(type: "double precision", nullable: false),
                    walk_from_dropoff_meters = table.Column<double>(type: "double precision", nullable: false),
                    shared_distance_meters = table.Column<double>(type: "double precision", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    inserted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_matches", x => x.id);
                    table.ForeignKey(
                        name: "fk_route_matches_driver_routes_driver_route_id",
                        column: x => x.driver_route_id,
                        principalTable: "driver_routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_route_matches_passenger_routes_passenger_route_id",
                        column: x => x.passenger_route_id,
                        principalTable: "passenger_routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_driver_routes_driver_id",
                table: "driver_routes",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_routes_vehicle_id",
                table: "driver_routes",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_passenger_routes_passenger_id",
                table: "passenger_routes",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_matches_driver_id",
                table: "route_matches",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_matches_driver_route_id_passenger_route_id",
                table: "route_matches",
                columns: new[] { "driver_route_id", "passenger_route_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_route_matches_passenger_id",
                table: "route_matches",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_matches_passenger_route_id",
                table: "route_matches",
                column: "passenger_route_id");

            migrationBuilder.Sql("CREATE INDEX ix_driver_routes_path_geography ON driver_routes USING gist ((path::geography));");
            migrationBuilder.Sql("CREATE INDEX ix_passenger_routes_origin_geography ON passenger_routes USING gist ((origin::geography));");
            migrationBuilder.Sql("CREATE INDEX ix_passenger_routes_destination_geography ON passenger_routes USING gist ((destination::geography));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_passenger_routes_destination_geography;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_passenger_routes_origin_geography;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_driver_routes_path_geography;");

            migrationBuilder.DropTable(
                name: "route_matches");

            migrationBuilder.DropTable(
                name: "driver_routes");

            migrationBuilder.DropTable(
                name: "passenger_routes");
        }
    }
}

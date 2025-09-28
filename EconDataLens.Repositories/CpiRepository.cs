using System.Data;
using EconDataLens.Core.Configuration;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Interfaces;
using EconDataLens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EconDataLens.Repositories;

public class CpiRepository : ICpiRepository
{
    private readonly EconDataLensDbContext _dbContext;

    public CpiRepository(EconDataLensDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertCpiAreaAsync(IAsyncEnumerable<CpiArea> areas, CancellationToken ct = default)
    {
        var conn = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await conn.BeginTransactionAsync(ct);

            // temp table
            await using (var cmd = new NpgsqlCommand("""
                                                     CREATE TEMP TABLE tmp_cpi_area (
                                                       area_code VARCHAR(4) PRIMARY KEY, 
                                                       area_name VARCHAR(80) NOT NULL
                                                     ) ON COMMIT DROP;
                                                     """, conn, tx))
            {
                await cmd.ExecuteNonQueryAsync(ct);
            }

            // SINGLE COPY session, streamed
            await using (var writer = await conn.BeginBinaryImportAsync(
                             "COPY tmp_cpi_area (area_code, area_name) FROM STDIN (FORMAT BINARY)", ct))
            {
                await foreach (var a in areas.WithCancellation(ct))
                {
                    await writer.StartRowAsync(ct);
                    await writer.WriteAsync(a.AreaCode, NpgsqlTypes.NpgsqlDbType.Text, ct);
                    await writer.WriteAsync(a.AreaName, NpgsqlTypes.NpgsqlDbType.Text, ct);
                }
                await writer.CompleteAsync(ct);
            }

            // one upsert
            await using (var upsert = new NpgsqlCommand("""
                                                        INSERT INTO cpi_area (area_code, area_name)
                                                        SELECT area_code, area_name FROM tmp_cpi_area
                                                        ON CONFLICT (area_code) DO UPDATE SET area_name = EXCLUDED.area_name;
                                                        """, conn, tx))
            {
                await upsert.ExecuteNonQueryAsync(ct);
            }

            await tx.CommitAsync(ct);
        });

        _dbContext.ChangeTracker.Clear();
    }


    public async Task UpsertCpiDataAsync(IAsyncEnumerable<CpiData> cpiData, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpsertCpiFootnotesAsync(IAsyncEnumerable<CpiFootnote> footnotes, CancellationToken ct = default)
    {
        var conn = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await conn.BeginTransactionAsync(ct);

            // temp table
            await using (var cmd = new NpgsqlCommand("""
                                                     CREATE TEMP TABLE tmp_cpi_footnote (
                                                        footnote_code VARCHAR(1) PRIMARY KEY,
                                                        footnote_text VARCHAR(100) NOT NULL
                                                        ) ON COMMIT DROP;
                                                     """, conn, tx))
            {
                await cmd.ExecuteNonQueryAsync(ct);
            }

            await using (var writer = await conn.BeginBinaryImportAsync(
                             "COPY tmp_cpi_footnote (footnote_code, footnote_text) FROM STDIN (FORMAT BINARY)", ct))
            {
                await foreach (var f in footnotes.WithCancellation(ct))
                {
                    await writer.StartRowAsync(ct);
                    await writer.WriteAsync(f.FootnoteCode, NpgsqlTypes.NpgsqlDbType.Text, ct);
                    await writer.WriteAsync(f.FootnoteText, NpgsqlTypes.NpgsqlDbType.Text, ct);
                }

                await writer.CompleteAsync(ct);
            }
            
            // one upsert
            await using (var upsert = new NpgsqlCommand("""
                                                        INSERT INTO cpi_footnote (footnote_code, footnote_text)
                                                        SELECT footnote_code, footnote_text FROM tmp_cpi_footnote
                                                        ON CONFLICT (footnote_code) DO UPDATE SET footnote_text = EXCLUDED.footnote_text;
                                                        """, conn, tx))
            {
                await upsert.ExecuteNonQueryAsync(ct);
            }
            
            await tx.CommitAsync(ct);

        });
        
        _dbContext.ChangeTracker.Clear();
    }

    public async Task UpsertCpiItemAsync(IAsyncEnumerable<CpiItem> cpiItems, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpsertCpiPeriodAsync(IAsyncEnumerable<CpiPeriod> periods, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpsertCpiSeriesAsync(IAsyncEnumerable<CpiSeries> series, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
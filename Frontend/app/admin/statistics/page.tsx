"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import api from "@/lib/api";

interface StatisticsDto {
  totalOrders: number;
  totalRevenue: number;
  averageOrderValue: number;
  ordersPerStatus: Record<string, number>;
  lastRefreshed: string | null;
}

const CACHE_MAX_AGE_MS = 5 * 60 * 1000;

function formatDateTime(value: string | null): string {
  if (!value) {
    return "Niciodata";
  }

  return new Date(value).toLocaleString();
}

export default function AdminStatisticsPage() {
  const [stats, setStats] = useState<StatisticsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadStatistics = async () => {
    setError(null);

    try {
      const response = await api.get<StatisticsDto>("/statistics");
      setStats(response.data);
    } catch {
      setError("Nu s-au putut incarca statisticile.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadStatistics();
  }, []);

  const handleRefresh = async () => {
    setRefreshing(true);
    setError(null);

    try {
      const response = await api.post<StatisticsDto>("/statistics/refresh");
      setStats(response.data);
    } catch {
      setError("Refresh-ul statisticilor a esuat.");
    } finally {
      setRefreshing(false);
    }
  };

  const isStale = useMemo(() => {
    if (!stats?.lastRefreshed) {
      return true;
    }

    return Date.now() - new Date(stats.lastRefreshed).getTime() > CACHE_MAX_AGE_MS;
  }, [stats?.lastRefreshed]);

  const statusEntries = useMemo(() => {
    return Object.entries(stats?.ordersPerStatus ?? {}).sort((left, right) => right[1] - left[1]);
  }, [stats?.ordersPerStatus]);

  const maxStatusCount = useMemo(() => {
    if (statusEntries.length === 0) {
      return 1;
    }

    return Math.max(...statusEntries.map((entry) => entry[1]), 1);
  }, [statusEntries]);

  if (loading) {
    return <div className="p-8 text-center">Incarcare statistici...</div>;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-emerald-50 to-cyan-50 p-6">
      <div className="max-w-6xl mx-auto space-y-6">
        <header className="flex flex-wrap justify-between items-center gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Admin Statistics</h1>
            <p className="text-slate-600">Singleton cache pentru statistici globale despre comenzi.</p>
          </div>
          <div className="flex gap-2">
            <Link href="/admin/products" className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">
              Produse
            </Link>
            <button
              onClick={handleRefresh}
              disabled={refreshing}
              className="px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 disabled:opacity-50"
            >
              {refreshing ? "Refresh..." : "Refresh"}
            </button>
          </div>
        </header>

        {error && <p className="text-rose-600 font-medium">{error}</p>}

        <div className="flex items-center gap-3">
          <span
            className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${
              isStale ? "bg-amber-100 text-amber-900" : "bg-emerald-100 text-emerald-900"
            }`}
          >
            {isStale ? "Cache vechi (>5 minute)" : "Cache proaspat"}
          </span>
          <span className="text-sm text-slate-600">Ultima actualizare: {formatDateTime(stats?.lastRefreshed ?? null)}</span>
        </div>

        <section className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <article className="bg-white rounded-2xl border border-slate-100 shadow-md p-4">
            <p className="text-sm text-slate-500">Total comenzi</p>
            <p className="text-3xl font-bold text-slate-900 mt-1">{stats?.totalOrders ?? 0}</p>
          </article>
          <article className="bg-white rounded-2xl border border-slate-100 shadow-md p-4">
            <p className="text-sm text-slate-500">Total venituri</p>
            <p className="text-3xl font-bold text-slate-900 mt-1">{(stats?.totalRevenue ?? 0).toFixed(2)} RON</p>
          </article>
          <article className="bg-white rounded-2xl border border-slate-100 shadow-md p-4">
            <p className="text-sm text-slate-500">Medie comanda</p>
            <p className="text-3xl font-bold text-slate-900 mt-1">{(stats?.averageOrderValue ?? 0).toFixed(2)} RON</p>
          </article>
          <article className="bg-white rounded-2xl border border-slate-100 shadow-md p-4">
            <p className="text-sm text-slate-500">Status-uri distincte</p>
            <p className="text-3xl font-bold text-slate-900 mt-1">{statusEntries.length}</p>
          </article>
        </section>

        <section className="bg-white rounded-2xl border border-slate-100 shadow-md p-5 space-y-4">
          <h2 className="text-xl font-semibold text-slate-900">Comenzi per status</h2>

          {statusEntries.length === 0 && (
            <p className="text-slate-600">Nu exista comenzi pentru calculul statisticilor.</p>
          )}

          <div className="space-y-3">
            {statusEntries.map(([status, count]) => {
              const widthPercent = (count / maxStatusCount) * 100;

              return (
                <div key={status} className="space-y-1">
                  <div className="flex justify-between text-sm">
                    <span className="font-medium text-slate-700">{status}</span>
                    <span className="text-slate-500">{count}</span>
                  </div>
                  <div className="h-3 rounded-full bg-slate-100 overflow-hidden">
                    <div
                      className="h-full bg-gradient-to-r from-cyan-500 to-emerald-500"
                      style={{ width: `${widthPercent}%` }}
                    />
                  </div>
                </div>
              );
            })}
          </div>
        </section>
      </div>
    </div>
  );
}

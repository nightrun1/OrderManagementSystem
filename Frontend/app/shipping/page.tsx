"use client";

import { useState } from "react";
import Link from "next/link";
import api from "@/lib/api";

interface ShippingQuote {
  provider: string;
  price: number;
  estimatedDays: number;
  serviceType: string;
}

interface TrackingInfo {
  trackingNumber: string;
  status: string;
  location: string;
  lastUpdate: string;
}

export default function ShippingPage() {
  const [fromCity, setFromCity] = useState("");
  const [toCity, setToCity] = useState("");
  const [weightKg, setWeightKg] = useState("");
  const [quotes, setQuotes] = useState<ShippingQuote[]>([]);
  const [quotesLoading, setQuotesLoading] = useState(false);
  const [quotesError, setQuotesError] = useState<string | null>(null);

  const [trackProvider, setTrackProvider] = useState("FanCourier");
  const [trackingNumber, setTrackingNumber] = useState("");
  const [trackingInfo, setTrackingInfo] = useState<TrackingInfo | null>(null);
  const [trackLoading, setTrackLoading] = useState(false);
  const [trackError, setTrackError] = useState<string | null>(null);

  const [shipResult, setShipResult] = useState<string | null>(null);

  const fetchQuotes = async () => {
    setQuotesLoading(true);
    setQuotesError(null);
    try {
      const res = await api.get<ShippingQuote[]>("/shipping/quotes", {
        params: { fromCity, toCity, weightKg: parseFloat(weightKg) },
      });
      setQuotes(res.data);
    } catch {
      setQuotesError("Nu s-au putut incarca cotațiile.");
    } finally {
      setQuotesLoading(false);
    }
  };

  const createShipment = async (provider: string) => {
    try {
      const res = await api.post<{ trackingNumber: string }>("/shipping/create", {
        orderId: 0,
        providerName: provider,
        shippingAddress: `${fromCity} -> ${toCity}`,
      });
      setShipResult(`Expediere creata! Tracking: ${res.data.trackingNumber}`);
    } catch {
      setShipResult("Eroare la crearea expedierii.");
    }
  };

  const trackShipment = async () => {
    setTrackLoading(true);
    setTrackError(null);
    setTrackingInfo(null);
    try {
      const res = await api.get<TrackingInfo>("/shipping/track", {
        params: { provider: trackProvider, trackingNumber },
      });
      setTrackingInfo(res.data);
    } catch {
      setTrackError("Nu s-a putut urmari coletul.");
    } finally {
      setTrackLoading(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto p-6">
      <header className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Livrare (Adapter Pattern)</h1>
        <div className="flex gap-2">
          <Link href="/orders" className="bg-gray-600 hover:bg-gray-700 text-white px-3 py-2 rounded-lg">Comenzi</Link>
          <Link href="/cart" className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded-lg">Cos</Link>
        </div>
      </header>

      {/* Sectiunea Compara Preturi */}
      <section className="bg-white rounded-xl shadow-md p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">Compara preturi livrare</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
          <input
            type="text" placeholder="Oras origine" value={fromCity}
            onChange={(e) => setFromCity(e.target.value)}
            className="border rounded-lg px-3 py-2"
          />
          <input
            type="text" placeholder="Oras destinatie" value={toCity}
            onChange={(e) => setToCity(e.target.value)}
            className="border rounded-lg px-3 py-2"
          />
          <input
            type="number" placeholder="Greutate (kg)" value={weightKg}
            onChange={(e) => setWeightKg(e.target.value)}
            className="border rounded-lg px-3 py-2"
          />
        </div>
        <button
          onClick={fetchQuotes}
          disabled={quotesLoading || !fromCity || !toCity || !weightKg}
          className="bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg disabled:opacity-50"
        >
          {quotesLoading ? "Se incarca..." : "Compara preturi"}
        </button>

        {quotesError && <p className="text-red-500 mt-3">{quotesError}</p>}
        {shipResult && <p className="text-green-600 mt-3 font-medium">{shipResult}</p>}

        {quotes.length > 0 && (
          <table className="w-full mt-4 border-collapse">
            <thead>
              <tr>
                <th className="bg-gray-100 px-4 py-2 text-left">Provider</th>
                <th className="bg-gray-100 px-4 py-2 text-left">Pret</th>
                <th className="bg-gray-100 px-4 py-2 text-left">Zile estimate</th>
                <th className="bg-gray-100 px-4 py-2 text-left">Tip serviciu</th>
                <th className="bg-gray-100 px-4 py-2 text-left">Actiune</th>
              </tr>
            </thead>
            <tbody>
              {quotes.map((q) => (
                <tr key={q.provider}>
                  <td className="px-4 py-2 border-b">{q.provider}</td>
                  <td className="px-4 py-2 border-b">{q.price.toFixed(2)} RON</td>
                  <td className="px-4 py-2 border-b">{q.estimatedDays}</td>
                  <td className="px-4 py-2 border-b">{q.serviceType}</td>
                  <td className="px-4 py-2 border-b">
                    <button
                      onClick={() => createShipment(q.provider)}
                      className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded-lg"
                    >
                      Selecteaza
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {/* Sectiunea Urmarire Colet */}
      <section className="bg-white rounded-xl shadow-md p-6">
        <h2 className="text-xl font-semibold mb-4">Urmarire colet</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
          <select
            value={trackProvider}
            onChange={(e) => setTrackProvider(e.target.value)}
            className="border rounded-lg px-3 py-2"
          >
            <option value="FanCourier">FanCourier</option>
            <option value="DPD">DPD</option>
          </select>
          <input
            type="text" placeholder="Tracking number" value={trackingNumber}
            onChange={(e) => setTrackingNumber(e.target.value)}
            className="border rounded-lg px-3 py-2 md:col-span-2"
          />
        </div>
        <button
          onClick={trackShipment}
          disabled={trackLoading || !trackingNumber}
          className="bg-amber-600 hover:bg-amber-700 text-white px-4 py-2 rounded-lg disabled:opacity-50"
        >
          {trackLoading ? "Se cauta..." : "Track"}
        </button>

        {trackError && <p className="text-red-500 mt-3">{trackError}</p>}

        {trackingInfo && (
          <div className="mt-4 bg-gray-50 rounded-lg p-4">
            <p><span className="font-semibold">Tracking:</span> {trackingInfo.trackingNumber}</p>
            <p><span className="font-semibold">Status:</span> {trackingInfo.status}</p>
            <p><span className="font-semibold">Locatie:</span> {trackingInfo.location}</p>
            <p><span className="font-semibold">Ultima actualizare:</span> {new Date(trackingInfo.lastUpdate).toLocaleString()}</p>
          </div>
        )}
      </section>
    </div>
  );
}

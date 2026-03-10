"use client";

import { useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function DashboardPage() {
  const router = useRouter();
  const hasToken = typeof window !== "undefined" && Boolean(window.localStorage.getItem("token"));
  const user = { email: "user@example.com", fullName: "User" };

  useEffect(() => {
    if (!hasToken) {
      router.push("/login");
    }
  }, [hasToken, router]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    router.push("/login");
  };

  if (!hasToken) return null;

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50 to-pink-50">
      {/* Header */}
      <header className="border-b border-slate-200 bg-white/80 backdrop-blur-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">O</span>
              </div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">Dashboard</h1>
            </Link>
            <button 
              onClick={handleLogout}
              className="px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50 rounded-lg transition-colors"
            >
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* Welcome Section */}
        <div className="mb-12">
          <h2 className="text-4xl font-bold text-slate-900 mb-4">Welcome back, {user.fullName}!</h2>
          <p className="text-xl text-slate-600">Here is what is happening with your orders today.</p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
          <div className="bg-white rounded-2xl shadow-md p-6 border border-slate-100">
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center">
                <span className="text-2xl">📦</span>
              </div>
              <div>
                <p className="text-sm text-slate-600">Total Orders</p>
                <p className="text-3xl font-bold text-slate-900">12</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-2xl shadow-md p-6 border border-slate-100">
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center">
                <span className="text-2xl">✅</span>
              </div>
              <div>
                <p className="text-sm text-slate-600">Completed</p>
                <p className="text-3xl font-bold text-green-600">8</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-2xl shadow-md p-6 border border-slate-100">
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 bg-orange-100 rounded-xl flex items-center justify-center">
                <span className="text-2xl">⏳</span>
              </div>
              <div>
                <p className="text-sm text-slate-600">Pending</p>
                <p className="text-3xl font-bold text-orange-600">4</p>
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="mb-8">
          <h3 className="text-2xl font-bold text-slate-900 mb-6">Quick Actions</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <Link href="/products" className="group bg-white rounded-2xl shadow-md hover:shadow-xl p-6 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
              <div className="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                <span className="text-2xl">🛒</span>
              </div>
              <h4 className="text-lg font-bold text-slate-900 mb-2">Browse Products</h4>
              <p className="text-sm text-slate-600">Explore our catalog</p>
            </Link>

            <Link href="/orders" className="group bg-white rounded-2xl shadow-md hover:shadow-xl p-6 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
              <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                <span className="text-2xl">📋</span>
              </div>
              <h4 className="text-lg font-bold text-slate-900 mb-2">My Orders</h4>
              <p className="text-sm text-slate-600">Track your orders</p>
            </Link>

            <Link href="/admin/products" className="group bg-white rounded-2xl shadow-md hover:shadow-xl p-6 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
              <div className="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                <span className="text-2xl">⚙️</span>
              </div>
              <h4 className="text-lg font-bold text-slate-900 mb-2">Admin Panel</h4>
              <p className="text-sm text-slate-600">Manage inventory</p>
            </Link>

            <Link href="/" className="group bg-gradient-to-br from-blue-600 to-indigo-600 rounded-2xl shadow-md hover:shadow-xl p-6 transition-all duration-300 hover:-translate-y-1 text-white">
              <div className="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                <span className="text-2xl">🏠</span>
              </div>
              <h4 className="text-lg font-bold mb-2">Home</h4>
              <p className="text-sm text-blue-100">Back to homepage</p>
            </Link>
          </div>
        </div>
      </main>
    </div>
  );
}

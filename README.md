// src/pages/AdminUsers.jsx
import React, { useEffect, useState } from "react";
import axios from "axios";

const API = "https://localhost:7199";

// Try these in order until one succeeds (2xx) for generic registration
const REGISTER_ENDPOINTS = [
  "/api/User/register",
  "/api/Users/register",
  "/api/Account/register",
  "/api/Authentication/register",
  "/api/Auth/Register",
];

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [deletingId, setDeletingId] = useState(null);

  // form state
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("User");
  const [saving, setSaving] = useState(false);
  const [msg, setMsg] = useState("");

  // promotion in-progress flag (per row)
  const [promotingId, setPromotingId] = useState(null);

  const auth = () => {
    const token = localStorage.getItem("token");
    return token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  };

  const errText = (e) =>
    e?.response?.data
      ? (typeof e.response.data === "string"
          ? e.response.data
          : JSON.stringify(e.response.data))
      : (e?.message || "Something went wrong");

  async function loadUsers() {
    try {
      setLoading(true);
      setErr("");
      const res = await axios.get(`${API}/api/Users`, auth());
      setUsers(res.data || []);
    } catch (e) {
      setErr(errText(e));
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    (async () => { await loadUsers(); })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function deleteUser(id) {
    if (!window.confirm("Delete user?")) return;
    try {
      setDeletingId(id);
      await axios.delete(`${API}/api/Users/${id}`, auth());
      setUsers(prev => prev.filter(u => u.userId !== id));
    } catch (e) {
      alert(errText(e));
    } finally {
      setDeletingId(null);
    }
  }

  // Helper: generic register through any known endpoint
  async function registerViaFallbacks(body) {
    try {
      await axios.post(`${API}/api/Users`, body, auth());
      return true;
    } catch (firstErr) {
      let lastErr = firstErr;
      for (const ep of REGISTER_ENDPOINTS) {
        try {
          await axios.post(`${API}${ep}`, body, auth());
          return true;
        } catch (e2) {
          lastErr = e2;
        }
      }
      throw new Error(
        "Could not find a working register endpoint.\n" +
        "Tried: POST /api/Users and " + REGISTER_ENDPOINTS.join(", ")
      );
    }
  }

  // === ADD USER / OPERATOR ===
  async function addUser(e) {
    e.preventDefault();
    setSaving(true);
    setMsg("");
    const body = { name, email, password, role };

    try {
      if (role === "BusOperator") {
        // Backend creates User(Role=BusOperator) + BusOperator linked to it
        await axios.post(`${API}/api/BusOperator`, { name, email, password }, auth());
        setMsg("✅ Created Bus Operator");
      } else {
        // Regular user (or Admin). Use existing register flow.
        await registerViaFallbacks(body);
        setMsg(`✅ Created ${role}`);
      }

      // reset form + refresh table
      setName(""); setEmail(""); setPassword(""); setRole("User");
      await loadUsers();

      // auto-hide
      setTimeout(() => setMsg(""), 2500);
    } catch (e) {
      alert(errText(e) || "Failed to create user.");
    } finally {
      setSaving(false);
    }
  }

  // === PROMOTE EXISTING USER → BUS OPERATOR ===
  const promoteToOperator = async (u) => {
    if (!window.confirm(`Make ${u.email} a Bus Operator?`)) return;
    try {
      setPromotingId(u.userId);

      // 1) Try without password (works if backend treats this as promotion)
      await axios.post(`${API}/api/BusOperator`, {
        name: u.name,
        email: u.email,
        userId: u.userId
      }, auth());

    } catch (e) {
      // 2) If backend requires a Password, retry with a temp one
      const data = e?.response?.data;
      const passwordRequired =
        (typeof data === "object" && data?.errors?.Password) ||
        (typeof data === "string" && /password/i.test(data));

      if (passwordRequired) {
        await axios.post(`${API}/api/BusOperator`, {
          name: u.name,
          email: u.email,
          userId: u.userId,
          password: "Temp#12345"
        }, auth());
      } else {
        console.error("Promote error:", e);
        alert(errText(e) || "Promotion failed");
        setPromotingId(null);
        return;
      }
    }

    setPromotingId(null);
    await loadUsers();
    setMsg("✅ Promoted to Bus Operator");
    setTimeout(() => setMsg(""), 2500);
  };

  return (
    <div style={{ padding: 40 }}>
      <h2 style={{ color: "#8e24aa", textAlign: "center" }}>Manage Users</h2>

      {/* Add User */}
      <form onSubmit={addUser} style={formRow}>
        <input
          style={input}
          placeholder="Name"
          value={name}
          onChange={e=>setName(e.target.value)}
          required
        />
        <input
          style={input}
          placeholder="Email"
          type="email"
          value={email}
          onChange={e=>setEmail(e.target.value)}
          required
        />
        <input
          style={input}
          placeholder="Password"
          type="password"
          value={password}
          onChange={e=>setPassword(e.target.value)}
          required
        />
        <select
          style={input}
          value={role}
          onChange={e=>setRole(e.target.value)}
        >
          <option value="User">User</option>
          <option value="BusOperator">BusOperator</option>
          <option value="Admin">Admin</option>
        </select>
        <button type="submit" disabled={saving} style={btn}>
          {saving ? "Saving..." : "Add"}
        </button>
      </form>

      {msg && <div style={{ color:"#239c4d", fontWeight:700, marginBottom:12 }}>{msg}</div>}

      {loading ? (
        <p>Loading...</p>
      ) : err ? (
        <p style={{color:'crimson'}}>{String(err)}</p>
      ) : (
        <table style={{ width: "100%", borderCollapse: "collapse" }}>
          <thead>
            <tr>
              <th style={th}>UserId</th>
              <th style={th}>Name</th>
              <th style={th}>Email</th>
              <th style={th}>Role</th>
              <th style={th}></th>
            </tr>
          </thead>
          <tbody>
            {users.map(u => (
              <tr key={u.userId}>
                <td style={td}>{u.userId}</td>
                <td style={td}>{u.name}</td>
                <td style={td}>{u.email}</td>
                <td style={td}>{u.role}</td>
                <td style={{...td, textAlign:"right"}}>
                  {u.role !== "BusOperator" && (
                    <button
                      style={{ marginRight: 8, opacity: promotingId===u.userId ? .6 : 1 }}
                      disabled={promotingId===u.userId}
                      onClick={() => promoteToOperator(u)}
                    >
                      {promotingId===u.userId ? "Promoting..." : "Promote to Operator"}
                    </button>
                  )}
                  <button onClick={() => deleteUser(u.userId)} disabled={deletingId===u.userId}>
                    {deletingId===u.userId ? "Deleting..." : "Delete"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

const th = { borderBottom: "1px solid #eee", padding: 12, textAlign: "left" };
const td = { borderBottom: "1px solid #f4f4f4", padding: 12 };
const formRow = { display:"grid", gridTemplateColumns:"1.1fr 1.2fr 1fr 0.9fr auto", gap:10, margin:"14px 0 22px" };
const input = { padding:"10px 12px", border:"1px solid #e9e9ef", borderRadius:10, outline:"none" };
const btn = { padding:"10px 14px", borderRadius:10, border:"1px solid #e9e9ef", background:"#f7c7e3", fontWeight:600 };
